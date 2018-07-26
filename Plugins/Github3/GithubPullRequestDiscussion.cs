﻿using System;
using System.Collections.Generic;
using System.Linq;
using Git.hub;
using GitUIPluginInterfaces.RepositoryHosts;

namespace Github3
{
    internal class GithubPullRequestDiscussion : IPullRequestDiscussion
    {
        private readonly PullRequest _pullRequest;

        public GithubPullRequestDiscussion(PullRequest pullRequest)
        {
            _pullRequest = pullRequest;
            Entries = new List<IDiscussionEntry>();
            ForceReload();
        }

        public List<IDiscussionEntry> Entries { get; private set; }

        public void Post(string data)
        {
            _pullRequest.ToIssue().CreateComment(data);
        }

        public void ForceReload()
        {
            Entries.Clear();

            foreach (var commit in _pullRequest.GetCommits())
            {
                Entries.Add(new GithubDiscussionCommit { Sha = commit.Sha, Author = commit.AuthorName.Replace("<", "&lt;").Replace(">", "&gt;"), Created = commit.Commit.Author.Date, Body = commit.Commit.Message });
            }

            foreach (var comment in _pullRequest.GetIssueComments())
            {
                Entries.Add(new GithubDiscussionComment { Author = comment.User.Login, Created = comment.CreatedAt, Body = comment.Body });
            }

            Entries = Entries.OrderBy(entry => entry.Created).ToList();
        }
    }

    internal class GithubDiscussionComment : IDiscussionEntry
    {
        public string Author { get; internal set; }
        public DateTime Created { get; internal set; }
        public string Body { get; internal set; }
    }

    internal class GithubDiscussionCommit : GithubDiscussionComment, ICommitDiscussionEntry
    {
        public string Sha { get; internal set; }
    }
}