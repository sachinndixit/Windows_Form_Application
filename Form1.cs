using ProjectWindowsForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Try
{
    public partial class Form1 : Form
    {
        //----PERSON TABLE VARIABLES----
        string str_firstName;
        string str_lastName;
        string str_mobile;
        string str_deptName;
        string str_role;

        //----SUBJECT TABLE VARIABLES----
        string str_subcode;
        string str_subname;
        string str_semester;
        int int_dept_ID;
        bool bool_common = false;
        bool bool_elective = false;

        //----ADDRESS TABLE VARIABLES----
        string str_address;
        string str_city;
        int int_city_ID;
        int int_person_Id;
        int int_return_PersonID;
        int int_return_AddressID;


        string str_sql;

        //----ROOM ALLOCATION VARIABLES----
        string str_buildingName;
        int int_roomNo;
        string str_FName;
        int Return_ID_For_allocation;
        string str_Role1;


        public Form1()
        {
            InitializeComponent();
            if (!clsSQLWrapper.s_blnHasConnection())
                MessageBox.Show("Error while connecting to database", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            //----CITIES FROM DATABASE----
            DataSet ds = clsSQLWrapper.runUserQuery("Select * from City");
            List<Model.clsCity> Cities = new List<Model.clsCity>();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                Parallel.For(0, ds.Tables[0].Rows.Count, i =>
                {
                    Model.clsCity city = new Model.clsCity();
                    city.CityName = ds.Tables[0].Rows[i]["City_Name"].ToString();
                    city.City_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["City_ID"]);
                    city.Country = ds.Tables[0].Rows[i]["Country"].ToString();
                    city.Pincode = Convert.ToInt32(ds.Tables[0].Rows[i]["Pincode"]);
                    lock (Cities)
                        Cities.Add(city);
                });
            }
            cmbCity.DataSource = Cities;
            cmbCity.DisplayMember = "CityName";
            cmbCity.ValueMember = "City_ID";


            //----DEPARTMENTS FROM DATABASE----
            DataSet ds1 = clsSQLWrapper.runUserQuery("Select * from Department");
            List<Model.clsDept> depts = new List<Model.clsDept>();
            if (ds1 != null && ds1.Tables[0].Rows.Count > 0)
            {
                Parallel.For(0, ds1.Tables[0].Rows.Count, i =>
                {
                    Model.clsDept dept = new Model.clsDept();
                    dept.Dept_ID = Convert.ToInt32(ds1.Tables[0].Rows[i]["Dept_ID"]);
                    dept.Dept_Name = ds1.Tables[0].Rows[i]["Dept_Name"].ToString();
                    lock (depts)
                        depts.Add(dept);
                });
            }
            cmbDept.DataSource = depts;
            cmbDept.DisplayMember = "Dept_Name";
            cmbDept.ValueMember = "Dept_ID";

            cmbDepartment.DataSource= depts;
            cmbDepartment.DisplayMember = "Dept_Name";
            cmbDepartment.ValueMember = "Dept_ID";

            //----SEMESTER FROM DATABASE----
            DataSet ds3 = clsSQLWrapper.runUserQuery("Select * from Semester");
            List<Model.clsSemester> Sems = new List<Model.clsSemester>();
            if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
            {
                Parallel.For(0, ds3.Tables[0].Rows.Count, i =>
                {
                    Model.clsSemester sem = new Model.clsSemester();
                    sem.Sem_ID = Convert.ToInt32(ds3.Tables[0].Rows[i]["Sem_ID"]);
                    sem.SemesterName = ds3.Tables[0].Rows[i]["Sem_Name"].ToString();

                    lock (Sems)
                        Sems.Add(sem);
                });
            }
            cmbSemester.DataSource = Sems;
            cmbSemester.DisplayMember = "SemesterName";
            cmbSemester.ValueMember = "Sem_ID";

            
        }

        private void tab_add_Click(object sender, EventArgs e)
        {


        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void tab_show_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Save_Button_Click(object sender, EventArgs e)
        {
            str_firstName = txtfirst.Text;
            str_lastName = txtlast.Text;
            str_address = txtaddress.Text;
            str_mobile = txtmob.Text;

            //----ROLES OF PERSON----
            if (rbtnStudent.Checked)
            {
                str_role = "Student";
            }
            else if (rbtnFaculty.Checked)
            {
                str_role = "Faculty";
            }
            else
            {
                str_role = "Administrator";
            }


            //----INSERT PERSON----

            if (txtfirst.Text != "" && txtlast.Text != "" && txtaddress.Text != "" && txtmob.Text != "" && cmbCity.SelectedValue != null && cmbDept.SelectedValue != null && (rbtnStudent.Checked == true || rbtnFaculty.Checked == true || rbtnAdministrator.Checked == true))
            {
                Regex reg = new Regex(@"^[0-9]{10}$");
                if (reg.IsMatch(txtmob.Text))
                {
                    if (clsSQLWrapper.s_blnHasConnection())
                    {
                        str_sql = "InsertPerson";
                        List<SqlParameter> lstPara = new List<SqlParameter>();
                        lstPara.Add(new SqlParameter { ParameterName = "@FirstName", SqlDbType = SqlDbType.VarChar, Value = str_firstName });
                        lstPara.Add(new SqlParameter { ParameterName = "@LastName", SqlDbType = SqlDbType.VarChar, Value = str_lastName });
                        lstPara.Add(new SqlParameter { ParameterName = "@Dept_ID", SqlDbType = SqlDbType.Int, Value = cmbDept.SelectedValue });
                        lstPara.Add(new SqlParameter { ParameterName = "@Role", SqlDbType = SqlDbType.VarChar, Value = str_role });
                        lstPara.Add(new SqlParameter { ParameterName = "@Phone_No", SqlDbType = SqlDbType.VarChar, Value = str_mobile });


                        int_return_PersonID = clsSQLWrapper.runProcedure(str_sql, lstPara);


                    }
                    str_address = txtaddress.Text;

                    int_person_Id = int_return_PersonID;


                    //----INSERT ADDRESS----
                    if (clsSQLWrapper.s_blnHasConnection())
                    {
                        str_sql = "InsertAddress";
                        List<SqlParameter> lstPara = new List<SqlParameter>();
                        lstPara.Add(new SqlParameter { ParameterName = "@Address", SqlDbType = SqlDbType.VarChar, Value = str_address });
                        lstPara.Add(new SqlParameter { ParameterName = "@City_ID", SqlDbType = SqlDbType.Int, Value = cmbCity.SelectedValue });
                        lstPara.Add(new SqlParameter { ParameterName = "@Person_ID", SqlDbType = SqlDbType.Int, Value = int_person_Id });
                        int_return_AddressID = clsSQLWrapper.runProcedure(str_sql, lstPara);
                    }


                    MessageBox.Show("Record Added Successfully");
                    func_clear_text();
                }
                else
                {
                    MessageBox.Show("Invalid Mobile No.");
                }
            }

            else
            {
                MessageBox.Show("Please,Enter all Required Fields");

            }
        }
        
        //----FUNCTION FOR CLEAR ALL FIELDS (ADDRESS)----
        public void func_clear_text()
        {
            txtfirst.Text = "";
            txtlast.Text = "";
            txtaddress.Text = "";
            txtmob.Text = "";
           

            cmbDept.DataSource = null;
            cmbCity.DataSource = null;
            

            rbtnStudent.Checked = false;
            rbtnFaculty.Checked = false;
            rbtnAdministrator.Checked = false;
            


        }


        //----FUNCTION FOR CLEAR ALL FIELDS (SUBJECTS)----
        public void func_clear_text1()
        {
           
            txtsubname.Text = "";
            textSubcode.Text = "";
            cmbSemester.DataSource = null;
            cmbDepartment.DataSource = null;
            rbtncommonYES.Checked = false;
            rbtncommonNO.Checked = false;
            rbtnelectiveYES.Checked = false;
            rbtnelectiveNO.Checked = false;

            
        }

        //----FUNCTION FOR CLEAR ALL FIELDS (ALLOCATION)----
        public void func_clear_text_allocation()
        {
            txtFname.Text = "";
            rbtn_Student.Checked = false;
            rbtn_Faculty.Checked = false;
            cmbBuilding.DataSource = null;
            cmbRoom.DataSource= null;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            func_display_details();
        }

        //----FUNCTION FOR DISPLAY DETAILS OF A PERSON----
        public void func_display_details()
        {
            //----PERSON TABLE-----
            string query;

            query = "select * from Person_Address inner join College_Person  " +
                "on Person_Address.Person_ID = College_Person.Person_ID " +
                "inner join City on City.City_ID = Person_Address.City_ID " +
                "left join Department  " +
                "on Department.Dept_ID = College_Person.Dept_ID;";
            DataSet dsData = clsSQLWrapper.runUserQuery(query);

            List<Model.clsPerson_Details> persons = new List<Model.clsPerson_Details>();
            Parallel.For(0, dsData.Tables[0].Rows.Count, i =>
            {
                Model.clsPerson_Details person = new Model.clsPerson_Details();
                Model.clsCity city = new Model.clsCity();
                Model.clsDept dept = new Model.clsDept();
                Model.clsAddress add = new Model.clsAddress();

                person.Person_ID = Convert.ToInt32(dsData.Tables[0].Rows[i]["Person_ID"]);
                person.FirstName = dsData.Tables[0].Rows[i]["FirstName"].ToString();
                person.LastName = dsData.Tables[0].Rows[i]["LastName"].ToString();
                dept.Dept_ID = Convert.ToInt32(dsData.Tables[0].Rows[i]["Dept_ID"]);
                dept.Dept_Name = (dsData.Tables[0].Rows[i]["Dept_Name"]).ToString();
                person.PersonDept = dept.Dept_Name;
                person.Role = dsData.Tables[0].Rows[i]["Role"].ToString();
                person.Phone_No = dsData.Tables[0].Rows[i]["Phone_No"].ToString();
                add.Add_ID = Convert.ToInt32(dsData.Tables[0].Rows[i]["Add_ID"]);
                add.Address = (dsData.Tables[0].Rows[i]["Address"]).ToString();
                city.City_ID = Convert.ToInt32(dsData.Tables[0].Rows[i]["City_ID"]);

                city.CityName = (dsData.Tables[0].Rows[i]["City_Name"]).ToString();
                city.Pincode = Convert.ToInt32(dsData.Tables[0].Rows[i]["Pincode"]);
                city.Country = (dsData.Tables[0].Rows[i]["Country"]).ToString();
                add.City = city;
                person.PersonFullAddress = add.Address + " " + city.CityName + " " + city.Country + " " + city.Pincode;
                lock (persons)
                    persons.Add(person);
            });
            
            dataGridView1.DataSource = persons.OrderBy(x => x.Person_ID).ToList();
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tab_sub_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnADD_Click(object sender, EventArgs e)
        {
            str_subcode = textSubcode.Text;
            str_subname = txtsubname.Text;


            groupBox1.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked);

            if (rbtncommonYES.Checked)
            {
                bool_common = true;
            }

            groupBox2.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked);

            if (rbtnelectiveYES.Checked)
            {
                bool_elective = true;
            }

            if ((str_subcode != "") && (str_subname != "") && (cmbDepartment.SelectedValue!=null) && (cmbSemester.SelectedValue != null) && ((rbtncommonYES.Checked==true) || (rbtncommonNO.Checked==true)) && ((rbtnelectiveYES.Checked==true) || (rbtnelectiveNO.Checked==true)))
            {
                Regex subject_code = new Regex(@"^[A-Z]{3}[-][0-9]{3}$");
                if (subject_code.IsMatch(textSubcode.Text))
                {
                    if (clsSQLWrapper.s_blnHasConnection())
                    {
                        string sql1 = "InsertSubject";
                        List<SqlParameter> lstSPara = new List<SqlParameter>();
                        lstSPara.Add(new SqlParameter { ParameterName = "@Sub_Code", SqlDbType = SqlDbType.VarChar, Value = str_subcode });
                        lstSPara.Add(new SqlParameter { ParameterName = "@Sub_Name", SqlDbType = SqlDbType.VarChar, Value = str_subname });
                        lstSPara.Add(new SqlParameter { ParameterName = "@IsElective", SqlDbType = SqlDbType.Bit, Value = bool_elective });
                        lstSPara.Add(new SqlParameter { ParameterName = "@Semester", SqlDbType = SqlDbType.Int, Value = cmbSemester.SelectedValue });
                        lstSPara.Add(new SqlParameter { ParameterName = "@IsCommonForAll", SqlDbType = SqlDbType.Bit, Value = bool_common });
                        lstSPara.Add(new SqlParameter { ParameterName = "@Department", SqlDbType = SqlDbType.Int, Value = cmbDepartment.SelectedValue });


                        clsSQLWrapper.runProcedure(sql1, lstSPara);




                        MessageBox.Show("Subject Added Successfully");
                        func_clear_text1();
                    }
                }
                else
                {
                    MessageBox.Show("Enter valid subject code: \ne.g ABC-123");
                }
            }
            else
            {
                MessageBox.Show("Enter Required Fields");
            }
        }

        public void func_display_Subject()
        {
            //----SUBJECT TABLE-----
            string QueryForSubject;

            
            QueryForSubject = "Select Sub_Code,Department.Dept_ID,Sub_Name,Dept_Name,Sem_Name,IsElective,IsCommon_For_All from Subjects inner join Department on Subjects.Dept_ID = Department.Dept_ID inner join Semester on Semester.Sem_ID = Subjects.Sem_ID;";

            DataSet dsData2 = clsSQLWrapper.runUserQuery(QueryForSubject);

            List<Model.clsSubjects> subject = new List<Model.clsSubjects>();
            Parallel.For(0, dsData2.Tables[0].Rows.Count, i =>
            {
                Model.clsPerson_Details person1 = new Model.clsPerson_Details();
                Model.clsDept dept1 = new Model.clsDept();
                Model.clsSubjects sub = new Model.clsSubjects();
                Model.clsSemester semester = new Model.clsSemester();

                sub.Sub_Code = dsData2.Tables[0].Rows[i]["Sub_Code"].ToString();
                sub.Sub_Name = dsData2.Tables[0].Rows[i]["Sub_Name"].ToString();
                sub.IsElective = Convert.ToBoolean(dsData2.Tables[0].Rows[i]["IsElective"]);
               
                dept1.Dept_ID = Convert.ToInt32(dsData2.Tables[0].Rows[i]["Dept_ID"]);
                dept1.Dept_Name= dsData2.Tables[0].Rows[i]["Dept_Name"].ToString();
                
                
                semester.SemesterName = dsData2.Tables[0].Rows[i]["Sem_Name"].ToString();
                sub.IsCommonForAll = Convert.ToBoolean(dsData2.Tables[0].Rows[i]["IsCommon_For_All"]);
                sub.subSem = semester.SemesterName;
                sub.subDept = dept1.Dept_Name;
                


                lock (subject)
                    subject.Add(sub);
            });
            
            dataGridView2.DataSource = subject.OrderBy(x=>x.Sub_Name).ToList();
            

        }

        private void rbtnyes1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void cmbSemester_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtlast_TextChanged(object sender, EventArgs e)
        {

        }

        private void ShowSubjectsBtn_Click(object sender, EventArgs e)
        {
            func_display_Subject();
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            
            DataSet ds4 = clsSQLWrapper.runUserQuery("Select * from Hostel where Person_ID IS NULL");
            List<Model.clsHostel> Hostels = new List<Model.clsHostel>();
            if (ds4 != null && ds4.Tables[0].Rows.Count > 0)
            {
                Parallel.For(0, ds4.Tables[0].Rows.Count, i =>
                {
                    Model.clsHostel Hostel = new Model.clsHostel();
                    Hostel.Building_Name = (ds4.Tables[0].Rows[i]["Building_Name"]).ToString();
                    Hostel.RoomNo = Convert.ToInt32(ds4.Tables[0].Rows[i]["Room_No"]);
                    Hostel.Alotted_To = (ds4.Tables[0].Rows[i]["Alloted_To"]).ToString();
                    lock (Hostels)
                        Hostels.Add(Hostel);
                });
            }
            
            List<string> BuildingNameHostel = Hostels.Select(s=>s.Building_Name).Distinct().ToList();
            cmbBuilding.DataSource = BuildingNameHostel;
            //cmbBuilding.DisplayMember = "Building_Name";
            //cmbBuilding.ValueMember = "Building_Name";
            cmbRoom.DataSource = Hostels;
            cmbRoom.DisplayMember = "RoomNo";
            cmbRoom.ValueMember = "RoomNo";


        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            DataSet ds5 = clsSQLWrapper.runUserQuery("Select * from Faculty_Room where ID IS NULL");
            List<Model.clsFaculty> Faculty = new List<Model.clsFaculty>();
            if (ds5 != null && ds5.Tables[0].Rows.Count > 0)
            {
                Parallel.For(0, ds5.Tables[0].Rows.Count, i =>
                {
                    Model.clsFaculty faculty = new Model.clsFaculty();
               
                    faculty.Building_Name = (ds5.Tables[0].Rows[i]["Building_Name"]).ToString();
                    faculty.Room_No = Convert.ToInt32(ds5.Tables[0].Rows[i]["Room_No"]);
                    faculty.Alloted_TO = (ds5.Tables[0].Rows[i]["Alloted_To"]).ToString();
                    lock (Faculty)
                        Faculty.Add(faculty);
                });
            }
            List<string> BuildingNameFaculty = Faculty.Select(s=>s.Building_Name).Distinct().ToList();
            cmbBuilding.DataSource = BuildingNameFaculty;
           
            cmbRoom.DataSource = Faculty;
            cmbRoom.DisplayMember = "Room_No";
            cmbRoom.ValueMember = "Room_No";
        }

        private void RoomAllocation_Click(object sender, EventArgs e)
        {
            str_FName = txtFname.Text;
            


            //----ROOM ALLOCATION----


            if (str_FName != "" && cmbBuilding.SelectedValue != null && cmbRoom.SelectedValue != null)
            {

                if (clsSQLWrapper.s_blnHasConnection())
                {
                    if (rbtn_Student.Checked)
                        str_Role1 = "Student";
                    else if (rbtn_Faculty.Checked)
                        str_Role1 = "Faculty";
                    List<SqlParameter> lst1 = new List<SqlParameter>();
                    lst1.Add(new SqlParameter { ParameterName = "@fName", SqlDbType = SqlDbType.VarChar, Value = str_FName });
                    lst1.Add(new SqlParameter { ParameterName = "@role", SqlDbType = SqlDbType.VarChar, Value = str_Role1 });
                    Return_ID_For_allocation = clsSQLWrapper.runProcedure("Return_Person_IdForAllocation", lst1);

                }
                if (Return_ID_For_allocation != 0)
                {
                    if (clsSQLWrapper.s_blnHasConnection())
                    {

                        List<SqlParameter> lstHPara = new List<SqlParameter>();
                        lstHPara.Add(new SqlParameter { ParameterName = "@Person_Id", SqlDbType = SqlDbType.VarChar, Value = Return_ID_For_allocation });
                        lstHPara.Add(new SqlParameter { ParameterName = "@Building_Name", SqlDbType = SqlDbType.VarChar, Value = cmbBuilding.SelectedValue });
                        lstHPara.Add(new SqlParameter { ParameterName = "@RoomNo", SqlDbType = SqlDbType.Int, Value = cmbRoom.SelectedValue });

                        if (rbtn_Student.Checked)
                            clsSQLWrapper.runProcedure("Insert_Hostel", lstHPara);
                        else if (rbtn_Faculty.Checked)
                            clsSQLWrapper.runProcedure("Insert_Faculty", lstHPara);
                    }
                    MessageBox.Show("Room Allocation Successful");
                    func_clear_text_allocation();


                }
                else
                {
                    MessageBox.Show("Enter Correct Name");
                }


            }
            else
            {
                MessageBox.Show("Enter all Required Fields");
            }
        }

        public void func_Show_Hostel_Allocation()
        {
            //----HOSTEL ALLOCATION TABLE----
            string QueryForRoom;


            QueryForRoom = "select * from Hostel where Person_ID is not null";

            DataSet dsData3 = clsSQLWrapper.runUserQuery(QueryForRoom);

            List<Model.clsHostel> lstHostel = new List<Model.clsHostel>();
            Parallel.For(0, dsData3.Tables[0].Rows.Count, i =>
            {
                Model.clsPerson_Details person2 = new Model.clsPerson_Details();
                Model.clsHostel hostelroom = new Model.clsHostel();

                hostelroom.Building_Name = dsData3.Tables[0].Rows[i]["Building_Name"].ToString();
                hostelroom.RoomNo = Convert.ToInt32(dsData3.Tables[0].Rows[i]["Room_No"]);
                hostelroom.Alotted_To = dsData3.Tables[0].Rows[i]["Alloted_to"].ToString();
                person2.Person_ID = Convert.ToInt32(dsData3.Tables[0].Rows[i]["Person_ID"].ToString());
                hostelroom.Person_ID = person2.Person_ID;

                lock (lstHostel)
                    lstHostel.Add(hostelroom);
            });

            dataGridView3.DataSource = lstHostel.OrderBy(x => x.Person_ID).ToList();


        }

        public void func_Show_Faculty_Allocation()
        {
            //----FACULTY ALLOCATION TABLE-----
            string QueryForFacultyRoom;


            QueryForFacultyRoom = "select * from Faculty_Room where ID is not null";

            DataSet dsData4 = clsSQLWrapper.runUserQuery(QueryForFacultyRoom);

            List<Model.clsFaculty> lstFaculty = new List<Model.clsFaculty>();
            Parallel.For(0, dsData4.Tables[0].Rows.Count, i =>
            {
                Model.clsPerson_Details person4 = new Model.clsPerson_Details();
                Model.clsFaculty facultyroom = new Model.clsFaculty();

                facultyroom.Building_Name = dsData4.Tables[0].Rows[i]["Building_Name"].ToString();
                facultyroom.Room_No = Convert.ToInt32(dsData4.Tables[0].Rows[i]["Room_No"]);
                facultyroom.Alloted_TO = dsData4.Tables[0].Rows[i]["Alloted_to"].ToString();
                person4.Person_ID = Convert.ToInt32(dsData4.Tables[0].Rows[i]["ID"]);
                facultyroom.PersonID = person4.Person_ID;

                lock (lstFaculty)
                    lstFaculty.Add(facultyroom);
            });

            dataGridView3.DataSource = lstFaculty.OrderBy(x => x.PersonID).ToList();


        }


        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ShowAllocatedRoom_Click(object sender, EventArgs e)
        {
            if(rbtn_Student.Checked)
            {
                func_Show_Hostel_Allocation();
            }
            else
            {
                func_Show_Faculty_Allocation();
            }
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
            