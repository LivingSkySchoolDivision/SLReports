﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Duplicates
{
    public partial class index : System.Web.UI.Page
    {
        private TableRow addStudentRowWithTreaty(Student student)
        {
            TableRow newRow = new TableRow();

            TableCell newCell = null;

            newCell = new TableCell();
            newCell.Text = student.getStudentID();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getFirstName();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getLegalMiddleName();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getLastName();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getGovernmentID();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getStatusNo();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getGender();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getDateOfBirth().ToShortDateString();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getSchoolName();
            newRow.Cells.Add(newCell);


            return newRow;
        }

        private TableRow addStudentRow(Student student)
        {
            TableRow newRow = new TableRow();

            TableCell newCell = null;

            newCell = new TableCell();
            newCell.Text = student.getStudentID();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getFirstName();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getLegalMiddleName();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getLastName();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getGovernmentID();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getGender();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getDateOfBirth().ToShortDateString();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getSchoolName();
            newRow.Cells.Add(newCell);


            return newRow;
        }

        private TableRow addStudentRowWithReason(Student student, String reason)
        {
            TableRow newRow = new TableRow();

            TableCell newCell = null;

            newCell = new TableCell();
            newCell.Text = student.getStudentID();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getFirstName();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getLegalMiddleName();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getLastName();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getGovernmentID();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getGender();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getDateOfBirth().ToShortDateString();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = student.getSchoolName();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = reason;
            newRow.Cells.Add(newCell);


            return newRow;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> allStudents = new List<Student>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allStudents = Student.loadAllStudents(connection);
            }

            Dictionary<string, Student> workspace_govid = new Dictionary<string, Student>();
            Dictionary<string, Student> workspace_names = new Dictionary<string, Student>();
            Dictionary<string, Student> workspace_treaty = new Dictionary<string, Student>();

            List<Student> foundDuplicates_govid = new List<Student>();
            List<Student> foundDuplicates_names = new List<Student>();
            List<Student> foundDuplicates_treaty = new List<Student>();

            Dictionary<Student, string> allDupesWithReason = new Dictionary<Student, string>();

            foreach (Student student in allStudents)
            {
                if ((!string.IsNullOrEmpty(student.getStatusNo())) && (student.getStatusNo().Length > 2))
                {                    
                    if (!workspace_treaty.ContainsKey(student.getStatusNo()))
                    {
                        workspace_treaty.Add(student.getStatusNo(), student);
                    }
                    else
                    {
                        foundDuplicates_treaty.Add(student);
                        foundDuplicates_treaty.Add(workspace_treaty[student.getStatusNo()]);

                        //allDupesWithReason.Add(student, "Same treaty number as " + workspace_treaty[student.getStatusNo()].getStudentID() + " (" + workspace_treaty[student.getStatusNo()].getDisplayName() + ")");
                        //allDupesWithReason.Add(workspace_treaty[student.getStatusNo()], "Same treaty number as " + student.getStudentID() + " (" + student.getDisplayName() + ")");
                    }
                }


                if (!string.IsNullOrEmpty(student.getGovernmentID()))
                {
                    if (!workspace_govid.ContainsKey(student.getGovernmentID()))
                    {
                        workspace_govid.Add(student.getGovernmentID(), student);
                    }
                    else
                    {
                        foundDuplicates_govid.Add(student);
                        foundDuplicates_govid.Add(workspace_govid[student.getGovernmentID()]);

                        allDupesWithReason.Add(student, "Same government ID as " + workspace_govid[student.getGovernmentID()].getStudentID() + " (" + workspace_govid[student.getGovernmentID()].getDisplayName() + ")");
                        allDupesWithReason.Add(workspace_govid[student.getGovernmentID()], "Same government ID as " + student.getStudentID() + " (" + student.getDisplayName() + ")");
                    }
                }


                String condensedName = student.getDisplayName().ToLower().Replace(" ",string.Empty).Replace("-", string.Empty);
                if (!workspace_names.ContainsKey(condensedName))
                {
                    workspace_names.Add(condensedName, student);
                }
                else
                {
                    foundDuplicates_names.Add(student);
                    foundDuplicates_names.Add(workspace_names[condensedName]);

                    if (student.getDateOfBirth().ToShortDateString() == workspace_names[condensedName].getDateOfBirth().ToShortDateString())
                    {
                        if (!allDupesWithReason.ContainsKey(student))
                        {
                            allDupesWithReason.Add(student, "Same name and date of birth as " + workspace_names[condensedName].getStudentID() + " (" + workspace_names[condensedName].getDisplayName() + ")");
                        }

                        if (!allDupesWithReason.ContainsKey(workspace_names[condensedName]))
                        {
                            allDupesWithReason.Add(workspace_names[condensedName], "Same name and date of birth as " + student.getStudentID() + " (" + student.getDisplayName() + ")");
                        }
                    }
                    else
                    {
                        if (!allDupesWithReason.ContainsKey(student))
                        {
                            allDupesWithReason.Add(student, "Same name as " + workspace_names[condensedName].getStudentID() + " (" + workspace_names[condensedName].getDisplayName() + ")");
                        }
                        if (!allDupesWithReason.ContainsKey(workspace_names[condensedName]))
                        {
                            allDupesWithReason.Add(workspace_names[condensedName], "Same name as " + student.getStudentID() + " (" + student.getDisplayName() + ")");
                        }
                         
                    }
                    
                }
            }

            foreach (Student student in foundDuplicates_govid)
            {
                tblGovIDs.Rows.Add(addStudentRow(student));
            }
            lblIdsCount.Text = "(" + foundDuplicates_govid.Count.ToString() + ")";

            foreach (Student student in foundDuplicates_names)
            {
                tblNames.Rows.Add(addStudentRow(student));
            }
            lblNamesCount.Text = "(" + foundDuplicates_names.Count.ToString() + ")";

            foreach (Student student in foundDuplicates_treaty)
            {
                tblTreaty.Rows.Add(addStudentRowWithTreaty(student));
            }
            lblTreaty.Text = "(" + foundDuplicates_treaty.Count.ToString() + ")";


            foreach (KeyValuePair<Student, String> student in allDupesWithReason)
            {
                tblAllDuplicates.Rows.Add(addStudentRowWithReason(student.Key, student.Value));
            }
            lblAllCount.Text = "(" + allDupesWithReason.Count.ToString() + ")";


            

        }
    }
}