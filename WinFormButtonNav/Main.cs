using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormButtonNav
{
    public partial class Main : Form
    {
        Main frmMain;
        public Main()
        {
            frmMain = this;
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            LoadButtonList();
        }

        List<Control> ButtonList;

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoadButtonList()
        {
            ButtonList = new List<Control>();
            // Button List must be sorted first by X axis then by Y axis for natural key nav to work
            ButtonList = Controls.Cast<Control>().Where(c=> c is Button || c is CheckBox).OrderBy(i=> i.Location.X).OrderBy(i=> i.Location.Y).ToList();
            //ButtonList = Controls.OfType<Button>().Cast<Control>().OrderBy(i => i.Location.Y).ToList();
            // needs to record Location X & Y

            //ButtonList.ForEach(c => c.KeyDown += new KeyEventHandler(Button_KeyDown));

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // See link below for source of this function
            // https://stackoverflow.com/a/34168026
            /*
                37	left arrow
                38	up arrow
                39	right arrow
                40	down arrow
             */

            var allControls = GetAllFormControls(frmMain);

            var current = GetFocusedControl(allControls, frmMain);
            var c = current.GetType();
            if (chbNavToggle.Checked && (current is Button || current is Form || current is CheckBox))
            {
                switch (keyData)
                {
                    case Keys.Left:
                        NextControl(ButtonList, "LEFT");
                        return true;
                    case Keys.Right:
                        NextControl(ButtonList, "RIGHT");
                        return true;
                    case Keys.Up:
                        NextControl(ButtonList, "UP");
                        return true;
                    case Keys.Down:
                        NextControl(ButtonList, "DOWN");
                        return true;
                    default:
                        break;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        public void Button_KeyDown(object sender = null, KeyEventArgs e = null)
        {
            
        }

        public List<Control> GetAllFormControls(Form myForm)
        {
            var myControls = myForm.Controls.Cast<Control>().ToList();
            return myControls;
        }

        public Control GetFocusedControl (List<Control> myControls, Form myForm)
        {
            var result = myControls.Where(c => c.Focused == true).FirstOrDefault();
            if (result == null)
            {
                if (myForm.Focused)
                {
                    result = myForm;
                }
            }
            return result;
        }

        public void NextControl (List<Control> myControls, string direction)
        {
            var elementNo = myControls.FindIndex(c => c.Focused == true);
            Control newControl = null;
            List<Control> newControls = null;
            var currentControl = myControls[elementNo] == null ? GetFocusedControl(myControls, frmMain): myControls[elementNo];
            switch (direction)
            {
                // Case UP is bugging
                case "LEFT":
                    newControls = myControls.Where(p => currentControl.Location.Y == p.Location.Y & currentControl.Location.X > p.Location.X).OrderByDescending(i => i.Location.X).ToList();
                    if (newControls.Count == 0)
                    {
                        newControls = myControls.Where(p => currentControl.Location.X > p.Location.X).OrderBy(i => Math.Abs(currentControl.Location.Y - i.Location.Y)).ToList();
                    }
                    break;
                case "RIGHT":
                    newControls = myControls.Where(p => currentControl.Location.Y == p.Location.Y & currentControl.Location.X < p.Location.X).OrderBy(i => i.Location.X).ToList();
                    if (newControls.Count == 0)
                    {
                        newControls = myControls.Where(p => currentControl.Location.X < p.Location.X).OrderBy(i => Math.Abs(currentControl.Location.Y - i.Location.Y)).ToList();
                    }
                    break;
                case "UP":
                    newControls = myControls.Where(p => currentControl.Location.X == p.Location.X & currentControl.Location.Y > p.Location.Y).OrderByDescending(i => i.Location.Y).ToList();
                    if (newControls.Count == 0)
                    {
                        newControls = myControls.Where(p => currentControl.Location.Y > p.Location.Y).OrderBy(i => Math.Abs(currentControl.Location.X - i.Location.X)).ToList();
                    }
                    
                    break;
                case "DOWN":
                    newControls = myControls.Where(p => currentControl.Location.X == p.Location.X & currentControl.Location.Y < p.Location.Y).OrderBy(i => i.Location.Y).ToList();
                    if (newControls.Count == 0)
                    {
                        //newControls = myControls.Where(p => currentControl.Location.Y < p.Location.Y).OrderBy(i => i.Location.Y).ToList();
                        newControls = myControls.Where(p => currentControl.Location.Y < p.Location.Y).OrderBy(i => Math.Abs(currentControl.Location.X - i.Location.X)).ToList();
                    }
                    break;
                default:
                    break;
            }
            newControl = newControls.FirstOrDefault();
            if (newControl != null)
            {
                newControl.Focus();
            }
        }
        
    }
}
