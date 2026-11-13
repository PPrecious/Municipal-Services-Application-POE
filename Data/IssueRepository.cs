
using System;
using System.Collections.Generic;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    public static class IssueRepository
    {
        private static readonly List<Issue> issues =
            new List<Issue>();

        public static void AddIssue(Issue issue)
        {
            if (issue == null)
                throw new ArgumentNullException("issue");

            issues.Add(issue);
            ServiceRequestRepository.Instance.AddFromIssue(issue);
        }

        public static List<Issue> GetIssues()
        {
            return new List<Issue>(issues);
        }

        public static int GetIssueCount()
        {
            return issues.Count;
        }
    }
}
