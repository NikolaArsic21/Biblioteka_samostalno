using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Biblioteka_samostalno
{
    public partial class Form1 : Form
    {
        private List<Clan> clanovi = new List<Clan>();
        public Form1()
        {
            InitializeComponent();
            listView();
            popuniGrid();
        }

        private void popuniGrid()
        {
            try
            {
                using (SqlConnection konekcija = new SqlConnection(Konekcija.konString))
                {
                    string upit = @"SELECT i.ID, 
                                    c.BrClanske, 
                                    c.Ime, 
                                    c.Prezime,
                                    i.NaslovKnjige,
                                    i.PeriodIznajmljivanja
                    FROM Iznajmljivanja i INNER JOIN Clanovi c ON c.ClanID = i.ClanID";
                    SqlCommand komanda = new SqlCommand(upit, konekcija);
                    konekcija.Open();
                    SqlDataReader dr = komanda.ExecuteReader();
                    while (dr.Read())
                    {
                        Clan clan = new Clan
                        {
                            IznajmljivanjeID = (int)dr["ID"],
                            BrojClanske = (int)dr["BrClanske"],
                            Ime = dr["Ime"].ToString(),
                            Prezime = dr["Prezime"].ToString(),
                            NaslovKnjige = dr["NaslovKnjige"].ToString(),
                            PeriodIznajmljivanja = (int)dr["PeriodIznajmljivanja"]
                        };
                        clanovi.Add(clan);
                        dataGridView1.DataSource = clanovi;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Došlo je do greške: " + ex.Message);
            }
        }

        private void FiltrirajPoNaslovu(string naslovKnjige)
        {
            listView1.Items.Clear();
            int ukupnoDana = 0;

            try
            {
                using (SqlConnection konekcija = new SqlConnection(Konekcija.konString))
                {
                    string sql = @"
                    SELECT c.Ime, c.Prezime, i.PeriodIznajmljivanja
                    FROM Iznajmljivanja i
                    INNER JOIN Clanovi c ON c.ClanID = i.ClanID
                    WHERE i.NaslovKnjige = @naslov";

                    SqlCommand cmd = new SqlCommand(sql, konekcija);
                    cmd.Parameters.AddWithValue("@naslov", naslovKnjige);

                    konekcija.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        int period = (int)dr["PeriodIznajmljivanja"];
                        ukupnoDana += period;

                        ListViewItem item = new ListViewItem(dr["Ime"].ToString());
                        item.SubItems.Add(dr["Prezime"].ToString());
                        item.SubItems.Add(period.ToString());

                        listView1.Items.Add(item);
                    }
                }

                lblUkupanBrojDana.Text = $"Ukupan broj dana: {ukupnoDana}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Došlo je do greške: " + ex.Message);
            }
        }

        private void listView()
        {
            listView1.View = View.Details;
            listView1.Columns.Add("Ime", 80);
            listView1.Columns.Add("Prezime", 80);
            listView1.Columns.Add("Period (dana)", 100);
        }

        private void izadjiIzAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void iznajmiKnjiguToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmIznajmljivanje frm = new frmIznajmljivanje();
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                MessageBox.Show("Uspješno ste iznajmili knjigu!");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            FiltrirajPoNaslovu(checkBox1.Text);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            FiltrirajPoNaslovu(checkBox2.Text);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            FiltrirajPoNaslovu(checkBox3.Text);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            FiltrirajPoNaslovu(checkBox4.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ucitajIzvestajToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Tekstualne datoteke (*.txt)|*.txt|Sve datoteke (*.*)|*.*";
                ofd.Title = "Odaberite datoteku za učitavanje";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Učitavanje sadržaja datoteke u RichTextBox
                        richTextBox1.Text = System.IO.File.ReadAllText(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Greška prilikom učitavanja datoteke: " + ex.Message,
                                        "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
        }

        private void btnEksportuj_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show("Nema podataka za eksportovanje.", "Upozorenje",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Tekstualne datoteke (*.txt)|*.txt";
                sfd.Title = "Sačuvajte izveštaj";
                sfd.FileName = "Izvestaj.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName))
                        {
                            // Opcionalno: naslov kolona
                            string[] kolone = listView1.Columns
                                             .Cast<ColumnHeader>()
                                             .Select(c => c.Text)
                                             .ToArray();
                            sw.WriteLine(string.Join("\t", kolone));

                            // Podaci iz ListView-a
                            foreach (ListViewItem item in listView1.Items)
                            {
                                string[] podaci = item.SubItems
                                                      .Cast<ListViewItem.ListViewSubItem>()
                                                      .Select(sub => sub.Text)
                                                      .ToArray();
                                sw.WriteLine(string.Join("\t", podaci));
                            }
                        }

                        MessageBox.Show("Izveštaj je uspešno sačuvan.", "Uspeh",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Došlo je do greške prilikom čuvanja izveštaja: " + ex.Message,
                                        "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
