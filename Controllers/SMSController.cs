using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
//using Newtonsoft.Json;
using SMSAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SMSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SMSController : ControllerBase
    {
        private const string URL = "https://apisms.bongolive.africa/v1/send";
        private readonly SMSAPIDBContext _context;

        public SMSController(SMSAPIDBContext context)
        {
            _context = context;
        }

        // POST api/SMS
        [HttpPost]
        public ActionResult Post([FromBody] SMSInput value)
        {

            string DATA = JsonSerializer.Serialize(value);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(DATA);
            requestWriter.Close();

            try
            {
                request.PreAuthenticate = true;
                String username = "abb40ca70b0d4726";// "06c76e11ca598786";
                String password = "MmM5MThiYTNiOGJiZjhhOTk3NzFiNmE5MzBlMjk2NWQ5Njk1MjIzZmViZmU2YWEyZjZjMDM2ODk3YWY2M2E2OQ==";// "ZTM4ZDRkYzk3NjZkMjdjMGNhNmRhMmE4NmU3NGE3OGNkN2I0ZGNjY2MzNGU3MmI5ZmJiNmVjMDcxOGQxZDI2MA ==";
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);

                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                Console.Out.WriteLine(response);
                responseReader.Close();
                return Ok(response);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
                return NotFound();
            }
        }

        // POST: api/SMS/SMSCallback
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<SMSCallback>> SMSCallback([Bind("request_id,recipient_id,dest_addr,Status")] SMSCallback smsCallback)
        {
            _context.smsCallback.Add(smsCallback);
            await _context.SaveChangesAsync();


            return new OkObjectResult(smsCallback);
        }
    }
}
