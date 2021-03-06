﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Roadkill.Core.Configuration;
using Roadkill.Core.Database;

namespace Roadkill.Core.Plugins.BuiltIn
{
	public class ImagesInNewWindow : TextPlugin
	{
		public override string Id
		{
			get 
			{ 
				return "ImagesInNewWindow";	
			}
		}

		public override string Name
		{
			get
			{
				return "ImagesInNewWindow name";
			}
		}

		public override string Description
		{
			get
			{
				return "ImagesInNewWindow";
			}
		}

		public override string Version
		{

			get
			{
				return "1.0";
			}
		}

		public ImagesInNewWindow(ApplicationSettings applicationSettings, IRepository repository)
			: base(applicationSettings, repository)
		{
		}

		public override string GetHeadContent()
		{
			AddScriptWithHeadJS("imagesinnewwindow.js");
			return GetScriptHtmlWithHeadJS();
		}
	}
}