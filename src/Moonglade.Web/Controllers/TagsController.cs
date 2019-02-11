﻿using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moonglade.Core;
using Moonglade.Data;
using Moonglade.Model.Settings;

namespace Moonglade.Web.Controllers
{
    public partial class TagsController : MoongladeController
    {
        private readonly TagService _tagService;
        private readonly PostService _postService;

        public TagsController(MoongladeDbContext context,
            ILogger<TagsController> logger,
            IOptions<AppSettings> settings,
            IConfiguration configuration,
            IHttpContextAccessor accessor, TagService tagService, PostService postService)
            : base(context, logger, settings, configuration, accessor)
        {
            _tagService = tagService;
            _postService = postService;
        }

        [Route("tags")]
        public IActionResult Index()
        {
            var list = _tagService.GetTagCountList();
            return View(list);
        }

        [Route("tags/list/{normalizedName}")]
        public IActionResult List(string normalizedName)
        {
            ViewBag.ErrorMessage = string.Empty;

            var tag = _tagService.GetTag(normalizedName);

            var postResponse = _postService.GetPostsByTag(normalizedName.ToLower());
            if (!postResponse.IsSuccess)
            {
                return ServerError();
            }

            var posts = postResponse.Item.ToList();
            if (posts.Any())
            {
                ViewBag.CurrentTagName = tag.DisplayName;
                ViewBag.TitlePrefix = tag.DisplayName;
                return View(posts);
            }
            return NotFound();
        }
    }
}