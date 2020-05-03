using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using DreamPlace.Lib.Rx;
using Serilog.Core;

namespace Project53.New_arhtech.Http.Server
{
	public class ValuesController
	{
		// Logger logger = LogManager.GetCurrentClassLogger();
		[Route("api/Values/Get")]
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/values/5 
		// public string Get(int id)
		// {
		// 	return "value";
		// }
		//
		// [HttpPost]
		// [Route("api/Values/LogIn")]
		// public IHttpActionResult LogIn([FromUri] string token)
		// {
		// 	try
		// 	{
		// 		Registry<string>.OnNext(new RegistryEventArgs<string>(token), "LogIn");
		// 	}
		// 	catch (Exception ex)
		// 	{
		// 		// logger.WriteException(ex);
		// 		return new Content().ObjectContent(HttpStatusCode.InternalServerError, ex.Message);
		// 	}
		//
		// 	return Ok();
		// }
		//
		// [HttpPost]
		// [Route("api/Values/LogOut")]
		// public IHttpActionResult LogOut(UserInfoViewModel token)
		// {
		// 	try
		// 	{
		// 		Registry<string>.OnNext(new RegistryEventArgs<string>(), "LogOut");
		// 	}
		// 	catch (Exception ex)
		// 	{
		// 		logger.WriteException(ex);
		// 	}
		//
		// 	return Ok();
		// }
		//
		// public class D
		// {
		// 	public string Document { get; set; }
		// }
		// [HttpPost]
		// [Route("api/Values/CurrentDocument")]
		// public IHttpActionResult CurrentDocument([FromBody]D data)
		// {
		// 	try
		// 	{
		// 		Registry<string>.OnNext(new RegistryEventArgs<string>(data.Document), "CurrentDocument");
		// 	}
		// 	catch (Exception ex)
		// 	{
		// 		logger.WriteException(ex);
		// 		return StatusCode(HttpStatusCode.InternalServerError);
		// 	}
		//
		// 	return Ok();
		// }
		//
		// [HttpPost]
		// [Route("api/Values/RefreshUserInfo")]
		// public IHttpActionResult RefreshUserInfo()
		// {
		// 	try
		// 	{
		// 		Registry<string>.OnNext(new RegistryEventArgs<string>(null), "RefreshUserInfo");
		// 	}
		// 	catch (Exception ex)
		// 	{
		// 		logger.WriteException(ex);
		// 	}
		//
		// 	return Ok();
		// }
		//
		// // DELETE api/values/5 
		// public void Delete(int id)
		// {
		// }
	}
}