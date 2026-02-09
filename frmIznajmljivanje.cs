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

namespace Biblioteka_samostalno
{
    public partial class frmIznajmljivanje : Form
    {
        private BindingList<Clan> clanovi = new BindingList<Clan>();
        public frmIznajmljivanje()
        {
            InitializeComponent();
            puniCombo();
        }

        private bool Validacija()
        {
            bool ispravno = true;
            errorProvider1.Clear();

            if (string.IsNullOrWhiteSpace(tbBrojClanskeKarte.Text))
            {
                ispravno = false;
                errorProvider1.SetError(tbBrojClanskeKarte, "Unesite broj članske karte.");

            }
            if (string.IsNullOrWhiteSpace(tbIme.Text))
            {
                ispravno = false;
                errorProvider1.SetError(tbIme, "Unesite ime člana.");
            }
            if (string.IsNullOrWhiteSpace(tbPrezime.Text))
            {
                ispravno = false;
                errorProvider1.SetError(tbPrezime, "Unesite prezime člana.");
            }
            if (cmbNaslov.SelectedIndex == -1)
            {
                ispravno = false;
                errorProvider1.SetError(cmbNaslov, "Odaberite naslov knjige.");
            }
            if (numericUpDown1.Value < 7 || numericUpDown1.Value > 60)
            {
                ispravno = false;
                errorProvider1.SetError(numericUpDown1, "Period iznajmljivanja mora biti izmedju 7 i 60 dana");
            }
            return ispravno;
        }

        private void puniCombo()
        {
            try
            {
                using (SqlConnection konekcija = new SqlConnection(Konekcija.konString))
                {
                    string upit = @"SELECT DISTINCT NaslovKnjige FROM Iznajmljivanja";
                    SqlCommand komanda = new SqlCommand(upit, konekcija);
                    konekcija.Open();
                    SqlDataReader dr = komanda.ExecuteReader();
                    while (dr.Read())
                    {
                        Clan clan = new Clan
                        {
                            NaslovKnjige = dr["NaslovKnjige"].ToString()
                        };
                        clanovi.Add(clan);
                        cmbNaslov.DataSource = clanovi;
                        cmbNaslov.DisplayMember = "NaslovKnjige";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnOtkazi_Click(object sender, EventArgs e)
        {
            tbBrojClanskeKarte.Text = "";
            tbIme.Text = "";
            tbPrezime.Text = "";
            cmbNaslov.SelectedIndex = -1;
            numericUpDown1.Value = 7;

            frmIznajmljivanje.ActiveForm.DialogResult = DialogResult.Cancel;
        }

        private void btnPotvrdi_Click(object sender, EventArgs e)
        {
            if (!Validacija())
            {
                MessageBox.Show("Unesite ispravne podatke.");
                return;
            }

            // Unos podataka u bazu
            try
            {
                using (SqlConnection konekcija = new SqlConnection(Konekcija.konString))
                {
                    string upit = @"INSERT INTO Clanovi (BrClanske, Ime, Prezime) 
                                    VALUES (@brojClanskeKarte, @ime, @prezime);
                                    INSERT INTO Iznajmljivanja (ClanID, NaslovKnjige, PeriodIznajmljivanja)
                                    VALUES (@ID, @naslovKnjige, @periodIznajmljivanja)";

                    SqlCommand komanda = new SqlCommand(upit, konekcija);
                    komanda.Parameters.AddWithValue("@brojClanskeKarte", tbBrojClanskeKarte.Text);
                    komanda.Parameters.AddWithValue("@ime", tbIme.Text);
                    komanda.Parameters.AddWithValue("@prezime", tbPrezime.Text);
                    komanda.Parameters.AddWithValue("@naslovKnjige", cmbNaslov.SelectedItem.ToString());
                    komanda.Parameters.AddWithValue("@periodIznajmljivanja", (int)numericUpDown1.Value);
                    konekcija.Open();
                    komanda.ExecuteNonQuery();
                    MessageBox.Show("Podaci su uspješno uneseni.");
                    frmIznajmljivanje.ActiveForm.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Došlo je do greške: " + ex.Message);
            }
        }
    }
}
