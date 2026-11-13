
using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    public class ServiceRequestRepository
    {
        private static readonly ServiceRequestRepository instance =
            new ServiceRequestRepository();

        public static ServiceRequestRepository Instance
        {
            get { return instance; }
        }

        private readonly Dictionary<int, ServiceRequest> byId =
            new Dictionary<int, ServiceRequest>();

        private readonly BinarySearchTree<ServiceRequest> bst =
            new BinarySearchTree<ServiceRequest>();

        private readonly AvlTree<ServiceRequest> avl =
            new AvlTree<ServiceRequest>();

        private readonly RedBlackRequestIndex redBlack =
            new RedBlackRequestIndex();

        private readonly BasicTreeNode<string> categoryTree;
        private readonly BinaryTreeNode<string> statusTree;
        private readonly ServiceLocationGraph graph;

        private int nextId = 1001;

        private ServiceRequestRepository()
        {
            categoryTree = new BasicTreeNode<string>("Municipal Services");

            string[] categories =
            {
                "Roads",
                "Water and Utilities",
                "Electricity",
                "Sanitation",
                "Waste Management",
                "Street Lighting",
                "Public Safety",
                "Other"
            };

            foreach (string category in categories)
                categoryTree.AddChild(new BasicTreeNode<string>(category));

            statusTree = new BinaryTreeNode<string>("Service Request");
            statusTree.Left = new BinaryTreeNode<string>("Open");
            statusTree.Right = new BinaryTreeNode<string>("Closed");

            statusTree.Left.Left =
                new BinaryTreeNode<string>("Submitted");

            statusTree.Left.Right =
                new BinaryTreeNode<string>("In Progress");

            statusTree.Right.Left =
                new BinaryTreeNode<string>("Resolved");

            statusTree.Right.Right =
                new BinaryTreeNode<string>("Cancelled");

            graph = new ServiceLocationGraph();

            graph.AddRoad("Municipal Depot", "Central", 4);
            graph.AddRoad("Municipal Depot", "North", 7);
            graph.AddRoad("Central", "North", 3);
            graph.AddRoad("Central", "South", 6);
            graph.AddRoad("North", "East", 5);
            graph.AddRoad("South", "East", 4);
            graph.AddRoad("East", "West", 8);
            graph.AddRoad("South", "West", 5);

            Seed();
        }

        private void Seed()
        {
            AddRequest(
                "Resident A", "Water and Utilities", "Central",
                "Water leak near the main road.",
                "In Progress", "High", DateTime.Today.AddDays(-4));

            AddRequest(
                "Resident B", "Electricity", "North",
                "Streetlight not working.",
                "Submitted", "Medium", DateTime.Today.AddDays(-2));

            AddRequest(
                "Resident C", "Roads", "South",
                "Pothole requires repair.",
                "In Progress", "High", DateTime.Today.AddDays(-7));

            AddRequest(
                "Resident D", "Waste Management", "East",
                "Missed refuse collection.",
                "Resolved", "Low", DateTime.Today.AddDays(-10));

            AddRequest(
                "Resident E", "Public Safety", "West",
                "Damaged public-area lighting.",
                "Submitted", "High", DateTime.Today.AddDays(-1));

            AddRequest(
                "Resident F", "Water and Utilities", "North",
                "Low water pressure.",
                "Submitted", "Medium", DateTime.Today.AddDays(-3));
        }

        public bool IsValidCategory(string category)
        {
            return !string.IsNullOrWhiteSpace(category) &&
                   categoryTree.Contains(category.Trim());
        }

        public bool IsValidStatus(string status)
        {
            return !string.IsNullOrWhiteSpace(status) &&
                   statusTree.Contains(status.Trim());
        }

        public List<string> GetCategories()
        {
            return categoryTree.PreOrder().Skip(1).ToList();
        }

        public List<string> GetStatuses()
        {
            return new List<string>
            {
                "Submitted",
                "In Progress",
                "Resolved",
                "Cancelled"
            };
        }

        public ServiceRequest AddFromIssue(Issue issue)
        {
            if (issue == null)
                throw new ArgumentNullException("issue");

            string category = NormaliseCategory(issue.Category);

            if (!IsValidCategory(category))
                category = "Other";

            string location = string.IsNullOrWhiteSpace(issue.Location)
                ? "Not specified"
                : issue.Location.Trim();

            string description = string.IsNullOrWhiteSpace(issue.Description)
                ? "No description supplied."
                : issue.Description.Trim();

            ServiceRequest request = AddRequest(
                "Resident",
                category,
                location,
                description,
                "Submitted",
                DeterminePriority(category, description),
                issue.DateReported);

            request.ExternalReferenceNumber = issue.ReferenceNumber;

            return request;
        }

        private string NormaliseCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return "Other";

            string value = category.Trim();

            if (value.Equals("Water", StringComparison.OrdinalIgnoreCase))
                return "Water and Utilities";

            if (value.Equals("Waste", StringComparison.OrdinalIgnoreCase))
                return "Waste Management";

            if (value.Equals("Road", StringComparison.OrdinalIgnoreCase))
                return "Roads";

            if (value.Equals("Streetlights", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Street Lighting", StringComparison.OrdinalIgnoreCase))
                return "Street Lighting";

            return value;
        }

        private string DeterminePriority(string category, string description)
        {
            string text = (category + " " + description).ToLowerInvariant();

            if (text.Contains("danger") ||
                text.Contains("emergency") ||
                text.Contains("burst") ||
                text.Contains("exposed wire") ||
                text.Contains("flood") ||
                text.Contains("sewage overflow"))
                return "High";

            if (text.Contains("leak") ||
                text.Contains("pothole") ||
                text.Contains("electricity") ||
                text.Contains("water") ||
                text.Contains("streetlight"))
                return "Medium";

            return "Low";
        }

        private ServiceRequest AddRequest(
            string resident,
            string category,
            string location,
            string description,
            string status,
            string priority,
            DateTime submitted)
        {
            if (!IsValidCategory(category))
                throw new ArgumentException("Invalid service category.");

            status = status == null ? null : status.Trim();

            if (!IsValidStatus(status))
                throw new ArgumentException("Invalid request status.");

            ServiceRequest request = new ServiceRequest
            {
                RequestId = nextId++,
                ResidentName = resident,
                Category = category,
                Location = location,
                Description = description,
                Status = status,
                Priority = priority,
                DateSubmitted = submitted,
                DateUpdated = submitted
            };

            byId.Add(request.RequestId, request);

            bst.Insert(request.RequestId, request);
            avl.Insert(request.RequestId, request);
            redBlack.Insert(request);

            return request;
        }

        public ServiceRequest TrackRequest(int id)
        {
            ServiceRequest request;
            return byId.TryGetValue(id, out request) ? request : null;
        }

        public ServiceRequest TrackWithBst(int id)
        {
            return bst.Search(id);
        }

        public ServiceRequest TrackWithAvl(int id)
        {
            return avl.Search(id);
        }

        public ServiceRequest TrackWithRedBlack(int id)
        {
            return redBlack.Search(id);
        }

        public bool RedBlackTreeIsValid()
        {
            return redBlack.Validate();
        }

        public int RedBlackLeftRotations
        {
            get { return redBlack.LeftRotations; }
        }

        public int RedBlackRightRotations
        {
            get { return redBlack.RightRotations; }
        }

        public int RedBlackRecolourings
        {
            get { return redBlack.Recolourings; }
        }

        public List<ServiceRequest> GetAll()
        {
            return byId.Values
                .OrderByDescending(r => r.DateSubmitted)
                .ThenBy(r => r.RequestId)
                .ToList();
        }

        public List<ServiceRequest> GetByPriority()
        {
            MinHeap<ServiceRequest> heap =
                new MinHeap<ServiceRequest>();

            foreach (ServiceRequest request in byId.Values)
            {
                int priority =
                    string.Equals(request.Priority, "High",
                        StringComparison.OrdinalIgnoreCase) ? 1 :
                    string.Equals(request.Priority, "Medium",
                        StringComparison.OrdinalIgnoreCase) ? 2 : 3;

                heap.Enqueue(request, priority);
            }

            List<ServiceRequest> result =
                new List<ServiceRequest>();

            while (heap.Count > 0)
                result.Add(heap.Dequeue());

            return result;
        }

        public void UpdateStatus(int id, string status)
        {
            status = status == null ? null : status.Trim();

            if (!IsValidStatus(status))
                throw new ArgumentException("Invalid request status.");

            ServiceRequest request = TrackRequest(id);

            if (request == null)
                throw new KeyNotFoundException("Request ID not found.");

            request.Status = status;
            request.DateUpdated = DateTime.Now;
        }

        public List<string> BFS()
        {
            return graph.BFS("Municipal Depot");
        }

        public List<string> DFS()
        {
            return graph.DFS("Municipal Depot");
        }

        public List<GraphEdge> MST(out int totalWeight)
        {
            return graph.MinimumSpanningTree(
                "Municipal Depot",
                out totalWeight);
        }

        public string GetStructureDiagnostics()
        {
            List<string> lines = new List<string>();

            lines.Add("1. BASIC TREE - CATEGORY VALIDATION");
            lines.Add("Valid Roads category: " + IsValidCategory("Roads"));
            lines.Add("Invalid category rejected: " + !IsValidCategory("Unknown"));

            lines.Add("");
            lines.Add("2. BINARY TREE - STATUS VALIDATION");
            lines.Add("Submitted accepted: " + IsValidStatus("Submitted"));
            lines.Add("In Progress accepted: " + IsValidStatus("In Progress"));
            lines.Add("Invalid status rejected: " + !IsValidStatus("Waiting"));

            lines.Add("");
            lines.Add("3. AVL TREE - FOUR ROTATION TESTS");

            TestAvlRotation(lines, "LL", new[] { 30, 20, 10 });
            TestAvlRotation(lines, "RR", new[] { 10, 20, 30 });
            TestAvlRotation(lines, "LR", new[] { 30, 10, 20 });
            TestAvlRotation(lines, "RL", new[] { 10, 30, 20 });

            lines.Add("");
            lines.Add("4. RED-BLACK TREE - INSERTION TESTS");

            RedBlackRequestIndex rotationTest =
                new RedBlackRequestIndex();

            int[] rotationIds = { 10, 5, 1, 15, 20 };

            foreach (int id in rotationIds)
            {
                rotationTest.Insert(new ServiceRequest
                {
                    RequestId = id,
                    Category = "Roads",
                    Status = "Submitted",
                    Priority = "High"
                });
            }

            lines.Add("Tree valid: " + rotationTest.Validate());
            lines.Add("Left rotations: " + rotationTest.LeftRotations);
            lines.Add("Right rotations: " + rotationTest.RightRotations);
            lines.Add("Recolourings: " + rotationTest.Recolourings);

            RedBlackRequestIndex recolourTest =
                new RedBlackRequestIndex();

            int[] recolourIds = { 10, 5, 15, 1 };

            foreach (int id in recolourIds)
            {
                recolourTest.Insert(new ServiceRequest
                {
                    RequestId = id,
                    Category = "Roads",
                    Status = "Submitted",
                    Priority = "Medium"
                });
            }

            lines.Add("Recolouring test valid: " + recolourTest.Validate());
            lines.Add("Recolouring operations: " +
                recolourTest.Recolourings);

            lines.Add("");
            lines.Add("5. PRIORITY MIN-HEAP TEST");

            MinHeap<ServiceRequest> testHeap =
                new MinHeap<ServiceRequest>();

            testHeap.Enqueue(
                new ServiceRequest { RequestId = 1, Priority = "Low" }, 3);

            testHeap.Enqueue(
                new ServiceRequest { RequestId = 2, Priority = "High" }, 1);

            testHeap.Enqueue(
                new ServiceRequest { RequestId = 3, Priority = "Medium" }, 2);

            while (testHeap.Count > 0)
            {
                ServiceRequest request = testHeap.Dequeue();

                lines.Add(
                    "Request " + request.RequestId +
                    " - " + request.Priority);
            }

            lines.Add("");
            lines.Add("6. LIVE REQUEST INDEX SEARCH");

            if (byId.Count > 0)
            {
                int id = byId.Keys.Min();

                lines.Add("BST search passed: " +
                    (TrackWithBst(id) != null));

                lines.Add("AVL search passed: " +
                    (TrackWithAvl(id) != null));

                lines.Add("Red-Black search passed: " +
                    (TrackWithRedBlack(id) != null));

                lines.Add("Red-Black tree valid: " +
                    redBlack.Validate());
            }
            else
            {
                lines.Add("No service requests are available to test.");
            }

            return string.Join(Environment.NewLine, lines);
        }

        private void TestAvlRotation(
            List<string> lines,
            string expected,
            int[] values)
        {
            AvlTree<int> testTree = new AvlTree<int>();

            foreach (int value in values)
                testTree.Insert(value, value);

            bool passed = testTree.LastRotation.ToString() == expected;

            lines.Add(
                expected + " rotation: " +
                (passed ? "Passed" : "Failed"));
        }
    }
}
