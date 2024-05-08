﻿using MediatR;
using Microsoft.Extensions.Logging;
using Moonglade.Data;
using Moonglade.Data.Entities;
using Moonglade.Data.Specifications;

namespace Moonglade.Comments;

public record DeleteCommentsCommand(Guid[] Ids) : IRequest;

public class DeleteCommentsCommandHandler(
    MoongladeRepository<CommentEntity> commentRepo, 
    ILogger<DeleteCommentsCommandHandler> logger) : IRequestHandler<DeleteCommentsCommand>
{
    public async Task Handle(DeleteCommentsCommand request, CancellationToken ct)
    {
        var spec = new CommentByIdsSepc(request.Ids);
        var comments = await commentRepo.ListAsync(spec, ct);
        foreach (var cmt in comments)
        {
            cmt.Replies.Clear();
            await commentRepo.DeleteAsync(cmt, ct);
        }

        logger.LogInformation("Deleted {Count} comment(s)", comments.Count);
    }
}