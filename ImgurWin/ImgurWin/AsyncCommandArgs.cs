using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImgurWin
{
	public class AsyncCommandArgs
	{
		public Resource resource;
		public object[] args;
		public AsyncCommandArgs(Resource resource, object[] args)
		{
			this.resource = resource;
			this.args = args;
		}
	}
}
