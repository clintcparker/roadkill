﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Roadkill.Core.Localization.Resx;

namespace Roadkill.Core.Mvc.ViewModels
{
	/// <summary>
	/// Provides summary data for a page.
	/// </summary>
	[CustomValidation(typeof(PageSummary), "VerifyRawTags")]
	public class PageSummary
	{
		private static string[] _tagBlackList = 
		{
			";", "/", "?", ":", "@", "&", "=", "{", "}", "|", "\\", "^", "[", "]", "`"		
		};

		private List<string> _tags;
		private string _rawTags;
		private string _content;

		/// <summary>
		/// The page's unique id.
		/// </summary>
		public int Id { get; set; }
		
		/// <summary>
		/// The text content for the page.
		/// </summary>
		public string Content
		{
			get { return _content; }
			set
			{
				// Ensure the content isn't null for lucene's benefit
				_content = value;
				if (_content == null)
					_content = "";
			}
		}

		/// <summary>
		/// The content after it has been transformed into HTML by the current wiki markup converter.
		/// </summary>
		public string ContentAsHtml { get; set; }

		/// <summary>
		/// The user who created the page.
		/// </summary>
		public string CreatedBy { get; set; }

		/// <summary>
		/// The date the page was created.
		/// </summary>
		public DateTime CreatedOn { get; set; }

		/// <summary>
		/// Returns true if no Id exists for the page.
		/// </summary>
		public bool IsNew
		{
			get
			{
				return Id == 0;
			}
		}

		/// <summary>
		/// The user who last modified the page.
		/// </summary>
		public string ModifiedBy { get; set; }

		/// <summary>
		/// The date the page was last modified on.
		/// </summary>
		public DateTime ModifiedOn { get; set; }
		
		/// <summary>
		/// The tags for the for the page.
		/// </summary>
		public IEnumerable<string> Tags
		{
			get { return _tags; }
		}

		/// <summary>
		/// The tags for the page - these are given by the page in comman separated format.
		/// </summary>
		public string RawTags
		{
			get 
			{ 
				return _rawTags; 
			}
			set
			{
				_rawTags = value;
				ParseRawTags();
			}
		}

		/// <summary>
		/// The page title before any update.
		/// </summary>
		public string PreviousTitle { get; set; }

		/// <summary>
		/// The page title.
		/// </summary>
		[Required(ErrorMessageResourceType=typeof(SiteStrings), ErrorMessageResourceName="Page_Validation_Title")]
		public string Title { get; set; }
		
		/// <summary>
		/// The current version number for the page.
		/// </summary>
		public int VersionNumber { get; set; }

		/// <summary>
		/// Whether the page has been locked so that only admins can edit it.
		/// </summary>
		public bool IsLocked { get; set; }

		/// <summary>
		/// Determines if the summary object can be cached on the browser and in the object cache. 
		/// This is true by default, but plugins that run on a page can mark a page as not cacheable.
		/// </summary>
		public bool IsCacheable { get; set; }

		/// <summary>
		/// Any additional head tag HTML generated by the custom variable plugins.
		/// </summary>
		public string PluginHeadHtml { get; set; }

		/// <summary>
		/// Any additional footer HTML generated by the custom variable plugins.
		/// </summary>
		public string PluginFooterHtml { get; set; }

		public PageSummary()
		{
			_tags = new List<string>();
			IsCacheable = true;
			PluginHeadHtml = "";
			PluginFooterHtml = "";
		}

		/// <summary>
		/// Joins the parsed tags with a comma.
		/// </summary>
		public string CommaDelimitedTags()
		{
			return string.Join(",", _tags);
		}

		/// <summary>
		/// Joins the tags with a space.
		/// </summary>
		public string SpaceDelimitedTags()
		{
			return string.Join(" ", _tags);
		}

		private void ParseRawTags()
		{
			_tags = ParseTags(_rawTags).ToList();
		}

		/// <summary>
		/// Takes a string of tags: "tagone,tagtwo,tag3 " and returns a list.
		/// </summary>
		/// <param name="tags"></param>
		/// <returns></returns>
		public static IEnumerable<string> ParseTags(string tags)
		{
			List<string> tagList = new List<string>();
			char delimiter = ',';

			if (!string.IsNullOrEmpty(tags))
			{
				// For the legacy tag seperator format
				if (tags.IndexOf(";") != -1)
					delimiter = ';';

				if (tags.IndexOf(delimiter) != -1)
				{
					string[] parts = tags.Split(delimiter);
					foreach (string item in parts)
					{
						if (item != ",")
							tagList.Add(item);
					}
				}
				else
				{
					tagList.Add(tags.TrimEnd());
				}
			}

			return tagList;
		}

		/// <summary>
		/// Returns false if the tag contains ; / ? : @ & = { } | \ ^ [ ] `		
		/// </summary>
		/// <param name="tag">The tag</param>
		/// <returns></returns>
		public static ValidationResult VerifyRawTags(PageSummary pageSummary, ValidationContext context)
		{
			if (string.IsNullOrEmpty(pageSummary.RawTags))
				return ValidationResult.Success;

			if (_tagBlackList.Any(x => pageSummary.RawTags.Contains(x)))
			{
				return new ValidationResult("Invalid characters in tag"); // doesn't need to be localized as there's a javascript check
			}
			else
			{
				return ValidationResult.Success;
			}
		}
	}
}
