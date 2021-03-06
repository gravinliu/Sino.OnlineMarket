﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Sino.OnlineMarket.Repositories.Repository;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Sino.OnlineMarket.Webhost.Controllers
{

    [Route("sino/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class PicturesController : Controller
    {
        private IHostingEnvironment hostingEnv;

        string[] pictureFormatArray = { "png", "jpg", "jpeg", "bmp", "gif", "ico", "PNG", "JPG", "JPEG", "BMP", "GIF", "ICO" };

        public PicturesController(IHostingEnvironment env)
        {
            this.hostingEnv = env;
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost("Post")]
        public IActionResult Post()
        {
            var files = Request.Form.Files;
            long size = files.Sum(f => f.Length);

            //size > 100MB refuse upload !
            if (size > 104857600)
            {
                return Json(PicturesRepository.Error_Msg_Ecode_Elevel_HttpCode("pictures total size > 100MB , server refused !"));
            }

            List<string> filePathResultList = new List<string>();

            foreach (var file in files)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                string filePath = hostingEnv.WebRootPath + $@"\Files\Pictures\";

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string suffix = fileName.Split('.')[1];

                if (!pictureFormatArray.Contains(suffix))
                {
                    return Json(PicturesRepository.Error_Msg_Ecode_Elevel_HttpCode("the picture format not support ! you must upload files that suffix like 'png','jpg','jpeg','bmp','gif','ico'."));
                }

                fileName = Guid.NewGuid() + "." + suffix;

                string fileFullName = filePath + fileName;

                using (FileStream fs = System.IO.File.Create(fileFullName))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                filePathResultList.Add($"/src/Sino.OnlineMarket.Webhost/wwwroot/FilesPictures/{fileName}");
            }

            string message = $"{files.Count} file(s) /{size} bytes uploaded successfully!";

            return Json(PicturesRepository.Success_Msg_Data_DCount_HttpCode(message, filePathResultList, filePathResultList.Count));
        }

    }
}
