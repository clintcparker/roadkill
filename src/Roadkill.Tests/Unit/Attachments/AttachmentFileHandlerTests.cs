﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Moq;
using NUnit.Framework;
using Roadkill.Core;
using Roadkill.Core.Attachments;
using Roadkill.Core.Configuration;
using Roadkill.Core.Converters;

namespace Roadkill.Tests.Unit
{
	[TestFixture]
	[Category("Unit")]
	public class AttachmentFileHandlerTests
	{
		private ApplicationSettings _applicationSettings;

		[SetUp]
		public void Setup()
		{
			_applicationSettings = new ApplicationSettings();
			_applicationSettings.AttachmentsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Attachments");
			_applicationSettings.AttachmentsRoutePath = "Attachments";
		}

		[Test]
		public void WriteResponse_Should_Set_200_Status_And_MimeType_And_Write_Bytes()
		{
			// Arrange
			AttachmentFileHandler handler = new AttachmentFileHandler(_applicationSettings);

			string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Attachments", "afile.jpg");
			File.WriteAllText(fullPath, "fake content");
			byte[] expectedBytes = File.ReadAllBytes(fullPath);
			string expectedMimeType = "image/jpeg";

			string localPath = "/wiki/Attachments/afile.jpg";
			string applicationPath = "/wiki";
			string modifiedSince = "";

			ResponseWrapperMock wrapper = new ResponseWrapperMock();

			// Act
			handler.WriteResponse(localPath, applicationPath, modifiedSince, wrapper);

			// Assert
			Assert.That(wrapper.StatusCode, Is.EqualTo(200));
			Assert.That(wrapper.ContentType, Is.EqualTo(expectedMimeType));
			Assert.That(wrapper.Buffer, Is.EqualTo(expectedBytes));
		}

		[Test]
		public void WriteResponse_Should_Set_404_Status_For_Bad_Application_Path()
		{
			// Arrange
			AttachmentFileHandler handler = new AttachmentFileHandler(_applicationSettings);

			string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Attachments", "afile.jpg");
			File.WriteAllText(fullPath, "fake content");

			string localPath = "/wiki/Attachments/afile.jpg";
			string applicationPath = "/wookie";
			string modifiedSince = "";

			ResponseWrapperMock wrapper = new ResponseWrapperMock();

			// Act
			handler.WriteResponse(localPath, applicationPath, modifiedSince, wrapper);

			// Assert
			Assert.That(wrapper.StatusCode, Is.EqualTo(404));
		}

		[Test]
		public void TranslateLocalPathToFilePath_Should_Be_Case_Sensitive()
		{
			// Arrange
			_applicationSettings.AttachmentsFolder = @"C:\attachments\";
			AttachmentFileHandler handler = new AttachmentFileHandler(_applicationSettings);

			// Act
			string actualPath = handler.TranslateUrlPathToFilePath("/Attachments/a.jpg", "/");

			// Assert
			Assert.That(actualPath, Is.Not.EqualTo(@"c:\Attachments\a.jpg"), "TranslateLocalPathToFilePath does a case sensitive url" +
																			 " replacement (this is for Apache compatibility");
		}

		[TestCase("/Attachments/a.jpg", "", @"C:\Attachments\a.jpg")] // should tolerate 'bad' application paths
		[TestCase("Attachments/a.jpg", "/", @"C:\Attachments\a.jpg")] // should tolerate url not beginning with /
		[TestCase("/Attachments/a.jpg", "/", @"C:\Attachments\a.jpg")]
		[TestCase("/Attachments/folder1/folder2/a.jpg", "/wiki/", @"C:\Attachments\folder1\folder2\a.jpg")]
		[TestCase("/wiki/Attachments/a.jpg", "/wiki/", @"C:\Attachments\a.jpg")]
		[TestCase("/wiki/Attachments/a.jpg", "/wiki", @"C:\Attachments\a.jpg")]
		[TestCase("/wiki/wiki2/Attachments/a.jpg", "/wiki/wiki2/", @"C:\Attachments\a.jpg")]
		public void TranslateLocalPathToFilePath(string localPath, string appPath, string expectedPath)
		{
			// Arrange
			_applicationSettings.AttachmentsFolder = @"C:\Attachments\";
			AttachmentFileHandler handler = new AttachmentFileHandler(_applicationSettings);

			// Act
			string actualPath = handler.TranslateUrlPathToFilePath(localPath, appPath);

			// Assert
			Assert.That(actualPath, Is.EqualTo(expectedPath), "Failed with {0} {1} {2}", localPath, appPath, expectedPath);
		}

		[Test]
		public void GetStatusCodeForCache_Should_Return_200_When_File_Was_Written_More_Recently()
		{
			// Arrange
			DateTime fileLastWritten = DateTime.Today;
			string ifModifiedSince = DateTime.Today.AddDays(-1).ToString("r"); // last time it was checked

			// Act
			int status = ResponseWrapper.GetStatusCodeForCache(fileLastWritten, ifModifiedSince); 

			// Assert
			Assert.That(status, Is.EqualTo(200));
		}

		[Test]
		public void GetStatusCodeForCache_Should_Return_304_When_File_Was_Checked_More_Recently()
		{
			// Arrange
			DateTime fileLastWritten = DateTime.Today.AddDays(-1);
			string ifModifiedSince = fileLastWritten.ToString("r"); // last modified stores the modified/write time of the file, i.e. it's exact

			// Act
			int status = ResponseWrapper.GetStatusCodeForCache(fileLastWritten, ifModifiedSince);

			// Assert
			Assert.That(status, Is.EqualTo(304));
		}
	}
}