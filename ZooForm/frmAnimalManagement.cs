﻿using BusinessObject.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZooForm
{
    public partial class frmAnimalManagement : Form
    {
        private IAnimalRepository _animalRepository = new AnimalRepository();

        private bool isCreating = false;

        private bool createOrUpdate;
        public frmAnimalManagement()
        {
            InitializeComponent();
        }

        private void frmAnimalManagement_Load(object sender, EventArgs e)
        {
            LoadAnimalData();
            EnableText(false);
        }
        private void LoadAnimalData()
        {
            BindingSource source = new BindingSource();
            source.DataSource = _animalRepository.GetAnimals();

            txtId.DataBindings.Clear();
            txtName.DataBindings.Clear();
            txtSpecies.DataBindings.Clear();
            txtLocation.DataBindings.Clear();
            txtClass.DataBindings.Clear();
            txtCreatedDate.DataBindings.Clear();
            radioTrue.DataBindings.Clear();

            txtId.DataBindings.Add("Text", source, "Id");
            txtName.DataBindings.Add("Text", source, "Name");
            txtSpecies.DataBindings.Add("Text", source, "Species");
            txtLocation.DataBindings.Add("Text", source, "Location");
            txtClass.DataBindings.Add("Text", source, "Class");
            txtCreatedDate.DataBindings.Add("Text", source, "CreatedDate");
            radioTrue.DataBindings.Add("Checked", source, "Status", true, DataSourceUpdateMode.OnPropertyChanged);

            dgv.DataSource = null;
            dgv.DataSource = source;

            dgv.Columns[2].Visible = false;
            radioTrue.Checked = false;
            radioFalse.Checked = true;
        }
        private void EnableText(bool status)
        {
            txtId.Enabled = status;
            txtName.Enabled = status;
            txtSpecies.Enabled = status;
            txtLocation.Enabled = status;
            txtClass.Enabled = status;
            txtCreatedDate.Enabled = status;
            radioTrue.Enabled = status;
            radioFalse.Enabled = status;
        }
        private void ClearFields()
        {
            // Clear the text in all input fields
            txtId.Clear();
            txtName.Clear();
            txtSpecies.Clear();
            txtLocation.Clear();
            txtClass.Clear();
            txtCreatedDate.Clear();
            radioTrue.Checked = false;
        }
        private void ClearFieldsDataBinding()
        {
            // Clear the text in all input fields
            txtId.DataBindings.Clear();
            txtName.DataBindings.Clear();
            txtSpecies.DataBindings.Clear();
            txtLocation.DataBindings.Clear();
            txtClass.DataBindings.Clear();
            txtCreatedDate.DataBindings.Clear();
            radioTrue.DataBindings.Clear();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (!isCreating)
            {
                // Switch to Create mode
                createOrUpdate = true;
                isCreating = true;
                ClearFieldsDataBinding();
                ClearFields();
                EnableText(true);
                btnCreate.Text = "Cancel";
            }
            else
            {
                // Switch back to normal mode
                ClearFields();
                isCreating = false;
                EnableText(false);
                btnCreate.Text = "Create";
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!isCreating)
            {
                // Switch to Update mode
                createOrUpdate = false;
                isCreating = true;
                EnableText(true);
                btnUpdate.Text = "Cancel";
            }
            else
            {
                // Switch back to normal mode
                isCreating = false;
                EnableText(false);
                btnUpdate.Text = "Update";
            }
        }
        private bool IsIdValid(int id)
        {
            // Get the list of existing animal IDs from your repository or data source
            List<int> existingIds = _animalRepository.GetAnimals().Select(a => a.Id).ToList();

            // Check if the provided ID already exists
            return existingIds.Contains(id);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            Animal animal = new Animal
            {
                // Assign values from the form controls to the user object
                Id = Convert.ToInt32(txtId.Text),
                Name = txtName.Text,
                Species = txtSpecies.Text,
                Location = txtLocation.Text,
                Class = txtClass.Text,

            };

            if (createOrUpdate)
            {
                animal.CreatedDate = DateTime.Now;
            }

            if (radioTrue.Checked)
            {
                animal.Status = true;
            }
            else
            {
                animal.Status = false;
            }

            // Determine whether to create or update based on the existence of the user ID
            if (createOrUpdate)
            {
                // Check if the ID already exists before saving
                if (IsIdValid(animal.Id))
                {
                    MessageBox.Show("Animal ID already exists. Please use a different ID.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return; // Stop further execution
                }

                _animalRepository.SaveAnimal(animal);
            }
            else
            {
                _animalRepository.UpdateAnimal(animal);
            }

            // Reload the data after saving
            LoadAnimalData();
            EnableText(false);

            if (createOrUpdate)
            {
                btnCreate.Text = "Create";
            }
            else
            {
                btnUpdate.Text = "Update";
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtId.Text))
            {
                // Confirm the deletion with the user
                DialogResult result = MessageBox.Show("Are you sure you want to delete this user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Delete the user
                    _animalRepository.DeleteAnimal(Convert.ToInt32(txtId.Text));

                    // Reload the data after deleting
                    LoadAnimalData();
                    EnableText(false);
                }
            }
            else
            {
                MessageBox.Show("Please select a user to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void radioTrue_CheckedChanged(object sender, EventArgs e)
        {
            if (radioTrue.Checked)
            {
                radioFalse.Checked = false;
            }
            else
            {
                radioFalse.Checked = true;
            }
        }

        private void radioFalse_CheckedChanged(object sender, EventArgs e)
        {
            if (radioFalse.Checked)
            {
                radioTrue.Checked = false;
            }
            else
            {
                radioTrue.Checked = true;
            }
        }

        private void txtId_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                List<Animal> animals = _animalRepository.GetAnimals();
                List<Animal> searchResult = animals.Where(a => a.Name.ToLower().Contains(searchTerm.ToLower())).ToList();

                // Display search results in DataGridView
                BindingSource source = new BindingSource();
                source.DataSource = searchResult;

                dgv.DataSource = null;
                dgv.DataSource = source;

                // Optionally, hide or show specific columns in the DataGridView if needed
                dgv.Columns[2].Visible = false; // Adjust column index as per your data

                // Clear data bindings for other fields
                ClearFieldsDataBinding();
            }
            else
            {
                // If the search term is empty, reload all animals
                LoadAnimalData();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            LoadAnimalData();
        }
    }
}
