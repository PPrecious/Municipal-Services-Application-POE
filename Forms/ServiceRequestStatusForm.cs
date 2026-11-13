
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Forms
{
    public partial class ServiceRequestStatusForm : Form
    {
        private readonly ServiceRequestRepository repository =
            ServiceRequestRepository.Instance;

        private DataGridView grid;
        private TextBox txtId;
        private Label lblDetails;
        private ComboBox cmbStatus;

        public ServiceRequestStatusForm()
        {
            BuildForm();
            LoadRequests();
        }

        private void BuildForm()
        {
            Text = "Service Request Status";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1100, 780);
            MinimumSize = new Size(950, 700);
            BackColor = Color.White;

            Label title = new Label
            {
                Text = "Service Request Status",
                Font = new Font("Arial", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 15)
            };

            Controls.Add(title);

            Label intro = new Label
            {
                Text = "View submitted municipal requests or track a request using its unique ID.",
                AutoSize = true,
                Location = new Point(22, 55)
            };

            Controls.Add(intro);

            Controls.Add(new Label
            {
                Text = "Request ID:",
                Location = new Point(22, 95),
                AutoSize = true
            });

            txtId = new TextBox
            {
                Location = new Point(100, 91),
                Width = 150
            };

            txtId.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Track(s, e);
                    e.SuppressKeyPress = true;
                }
            };

            Controls.Add(txtId);

            Button track = new Button
            {
                Text = "Track Request",
                Location = new Point(265, 88),
                Size = new Size(130, 30)
            };

            track.Click += Track;
            Controls.Add(track);

            Button all = new Button
            {
                Text = "Show All Requests",
                Location = new Point(405, 88),
                Size = new Size(145, 30)
            };

            all.Click += (s, e) => LoadRequests();
            Controls.Add(all);

            Button priority = new Button
            {
                Text = "Sort by Priority",
                Location = new Point(560, 88),
                Size = new Size(130, 30)
            };

            priority.Click += (s, e) =>
            {
                try
                {
                    Display(repository.GetByPriority());
                }
                catch (Exception ex)
                {
                    ShowError("Unable to sort requests by priority.", ex);
                }
            };

            Controls.Add(priority);

            grid = new DataGridView
            {
                Location = new Point(22, 140),
                Size = new Size(1035, 300),
                Anchor = AnchorStyles.Top |
                         AnchorStyles.Left |
                         AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowTemplate = { Height = 35 }
            };

            grid.Columns.Add("Id", "Request ID");
            grid.Columns.Add("Category", "Category");
            grid.Columns.Add("Location", "Location");
            grid.Columns.Add("Status", "Status");
            grid.Columns.Add("Priority", "Priority");
            grid.Columns.Add("Submitted", "Date Submitted");
            grid.Columns.Add("Description", "Description");

            grid.Columns["Id"].FillWeight = 65;
            grid.Columns["Category"].FillWeight = 85;
            grid.Columns["Location"].FillWeight = 100;
            grid.Columns["Status"].FillWeight = 80;
            grid.Columns["Priority"].FillWeight = 65;
            grid.Columns["Submitted"].FillWeight = 90;
            grid.Columns["Description"].FillWeight = 160;

            grid.SelectionChanged += SelectionChanged;

            Controls.Add(grid);

            Controls.Add(new Label
            {
                Text = "Request Details",
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(22, 460)
            });

            lblDetails = new Label
            {
                Location = new Point(22, 490),
                Size = new Size(500, 145),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8),
                AutoSize = false,
                Text = "Select a request or enter a request ID to track it."
            };

            Controls.Add(lblDetails);

            Controls.Add(new Label
            {
                Text = "Update status:",
                Location = new Point(555, 465),
                AutoSize = true
            });

            cmbStatus = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(555, 490),
                Width = 180
            };

            cmbStatus.Items.AddRange(new object[]
            {
                "Submitted",
                "In Progress",
                "Resolved",
                "Cancelled"
            });

            cmbStatus.SelectedIndex = 0;
            Controls.Add(cmbStatus);

            Button update = new Button
            {
                Text = "Update Selected Status",
                Location = new Point(750, 488),
                Size = new Size(190, 32)
            };

            update.Click += UpdateSelected;
            Controls.Add(update);

            Button graph = new Button
            {
                Text = "Show Graph Traversal and MST",
                Location = new Point(555, 535),
                Size = new Size(250, 35)
            };

            graph.Click += ShowGraph;
            Controls.Add(graph);

            Button diagnostics = new Button
            {
                Text = "Show Data Structure Diagnostics",
                Location = new Point(555, 580),
                Size = new Size(250, 35)
            };

            diagnostics.Click += ShowDiagnostics;
            Controls.Add(diagnostics);

            Button back = new Button
            {
                Text = "Back to Main Menu",
                Location = new Point(22, 665),
                Size = new Size(190, 38),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            back.Click += (s, e) => Close();
            Controls.Add(back);
        }

        private void LoadRequests()
        {
            try
            {
                Display(repository.GetAll());
            }
            catch (Exception ex)
            {
                ShowError("Unable to load service requests.", ex);
            }
        }

        private void Display(List<ServiceRequest> requests)
        {
            grid.Rows.Clear();

            if (requests == null || requests.Count == 0)
            {
                lblDetails.Text =
                    "No service requests are currently available.";

                return;
            }

            foreach (ServiceRequest request in requests)
            {
                if (request == null)
                    continue;

                int index = grid.Rows.Add(
                    request.RequestId,
                    request.Category,
                    request.Location,
                    request.Status,
                    request.Priority,
                    request.DateSubmitted.ToString("dd MMM yyyy"),
                    request.Description);

                grid.Rows[index].Tag = request;
            }

            if (grid.Rows.Count > 0)
            {
                grid.ClearSelection();
                grid.Rows[0].Selected = true;
                ShowDetails(grid.Rows[0].Tag as ServiceRequest);
            }
            else
            {
                lblDetails.Text =
                    "No service requests are currently available.";
            }
        }

        private void Track(object sender, EventArgs e)
        {
            int id;

            if (!int.TryParse(txtId.Text.Trim(), out id) || id <= 0)
            {
                MessageBox.Show(
                    "Enter a valid positive numeric request ID.",
                    "Invalid ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtId.Focus();
                return;
            }

            try
            {
                ServiceRequest bstResult =
                    repository.TrackWithBst(id);

                ServiceRequest avlResult =
                    repository.TrackWithAvl(id);

                ServiceRequest redBlackResult =
                    repository.TrackWithRedBlack(id);

                if (bstResult == null &&
                    avlResult == null &&
                    redBlackResult == null)
                {
                    MessageBox.Show(
                        "No request was found with ID " + id + ".",
                        "Request Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                if (bstResult == null ||
                    avlResult == null ||
                    redBlackResult == null)
                {
                    MessageBox.Show(
                        "The tree indexes returned incomplete results. " +
                        "At least one index could not find the request.",
                        "Index Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                if (bstResult.RequestId != avlResult.RequestId ||
                    bstResult.RequestId != redBlackResult.RequestId)
                {
                    MessageBox.Show(
                        "The tree indexes returned inconsistent results.",
                        "Index Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                Display(new List<ServiceRequest>
                {
                    redBlackResult
                });

                ShowDetails(redBlackResult);

                MessageBox.Show(
                    "Request found successfully using all three indexes:" +
                    Environment.NewLine +
                    "Binary Search Tree: Passed" +
                    Environment.NewLine +
                    "AVL Tree: Passed" +
                    Environment.NewLine +
                    "Red-Black Tree: Passed",
                    "Tree Search Results",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError("An error occurred while tracking the request.", ex);
            }
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0)
                return;

            ServiceRequest request =
                grid.SelectedRows[0].Tag as ServiceRequest;

            ShowDetails(request);
        }

        private void ShowDetails(ServiceRequest request)
        {
            if (request == null)
            {
                lblDetails.Text =
                    "Select a service request to view its details.";

                return;
            }

            lblDetails.Text =
                "Request ID: " + request.RequestId +
                Environment.NewLine +
                "Original reference: " +
                (request.ExternalReferenceNumber ?? "Demonstration request") +
                Environment.NewLine +
                "Resident: " + request.ResidentName +
                Environment.NewLine +
                "Category: " + request.Category +
                Environment.NewLine +
                "Location: " + request.Location +
                Environment.NewLine +
                "Status: " + request.Status +
                Environment.NewLine +
                "Priority: " + request.Priority +
                Environment.NewLine +
                "Submitted: " +
                request.DateSubmitted.ToString("dd MMMM yyyy") +
                Environment.NewLine +
                "Last updated: " +
                (request.DateUpdated.HasValue
                    ? request.DateUpdated.Value.ToString(
                        "dd MMMM yyyy HH:mm")
                    : "Not updated") +
                Environment.NewLine +
                "Description: " + request.Description;
        }

        private void UpdateSelected(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Select a request first.",
                    "No Request Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (cmbStatus.SelectedItem == null)
            {
                MessageBox.Show(
                    "Select a status before updating.",
                    "Status Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            ServiceRequest request =
                grid.SelectedRows[0].Tag as ServiceRequest;

            if (request == null)
            {
                MessageBox.Show(
                    "The selected request could not be retrieved.",
                    "Request Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            int requestId = request.RequestId;
            string newStatus = cmbStatus.SelectedItem.ToString();

            if (string.Equals(
                request.Status,
                newStatus,
                StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "The request already has this status.",
                    "No Change Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            try
            {
                repository.UpdateStatus(requestId, newStatus);

                List<ServiceRequest> updatedRequests =
                    repository.GetAll();

                Display(updatedRequests);

                foreach (DataGridViewRow row in grid.Rows)
                {
                    ServiceRequest updatedRequest =
                        row.Tag as ServiceRequest;

                    if (updatedRequest != null &&
                        updatedRequest.RequestId == requestId)
                    {
                        grid.ClearSelection();
                        row.Selected = true;
                        ShowDetails(updatedRequest);
                        break;
                    }
                }

                MessageBox.Show(
                    "The request status has been updated successfully.",
                    "Updated",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError("Unable to update the request status.", ex);
            }
        }

        private void ShowDiagnostics(object sender, EventArgs e)
        {
            try
            {
                string diagnostics =
                    repository.GetStructureDiagnostics();

                MessageBox.Show(
                    diagnostics,
                    "Data Structure Diagnostics",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError("Unable to run data structure diagnostics.", ex);
            }
        }

        private void ShowGraph(object sender, EventArgs e)
        {
            try
            {
                int totalWeight;

                List<GraphEdge> mst =
                    repository.MST(out totalWeight);

                List<string> edgeDescriptions =
                    new List<string>();

                foreach (GraphEdge edge in mst)
                {
                    edgeDescriptions.Add(edge.ToString());
                }

                string mstText = mst.Count == 0
                    ? "No spanning tree could be generated."
                    : string.Join(
                        Environment.NewLine,
                        edgeDescriptions);

                MessageBox.Show(
                    "BREADTH-FIRST SEARCH (BFS)" +
                    Environment.NewLine +
                    string.Join(" -> ", repository.BFS()) +

                    Environment.NewLine +
                    Environment.NewLine +

                    "DEPTH-FIRST SEARCH (DFS)" +
                    Environment.NewLine +
                    string.Join(" -> ", repository.DFS()) +

                    Environment.NewLine +
                    Environment.NewLine +

                    "MINIMUM SPANNING TREE (PRIM'S ALGORITHM)" +
                    Environment.NewLine +
                    mstText +

                    Environment.NewLine +
                    Environment.NewLine +
                    "Total MST weight: " + totalWeight,

                    "Municipal Location Network",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError("Unable to display graph traversal results.", ex);
            }
        }

        private void ShowError(string message, Exception ex)
        {
            MessageBox.Show(
                message + Environment.NewLine +
                Environment.NewLine +
                "Details: " + ex.Message,
                "Application Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
