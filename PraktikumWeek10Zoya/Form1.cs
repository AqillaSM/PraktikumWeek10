using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PraktikumWeek10Zoya
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        MySqlConnection sqlConnect = new MySqlConnection("server=localhost;uid=root;pwd=;database=premier_league"); // sebagai data koneksi ke DBMSnya, memasukkan IP Address, localhost, user, password
        MySqlCommand sqlCommand = new MySqlCommand("query kita"); // memindahkan query dari VS ke database
        MySqlDataAdapter sqlAdapter; // hasil query akan disimpan olehnya atau menjadi penampung
        string sqlQuery;

        private void Form1_Load(object sender, EventArgs e)
        {
            LabelManagerTM.Text = "";
            LabelManagerTR.Text = "";
            LabelCaptainTM.Text = "";
            LabelCaptainTR.Text = "";
            LabelCapacity.Text = "";
            LabelStadium.Text = "";
            LabelTanggal.Text = "";
            LabelSkor.Text = "";
            sqlConnect.Open();
            DataTable dtPlayerTR = new DataTable();
            DataTable dtPlayerTM = new DataTable();

            //copy mulai sini
            sqlQuery = "SELECT team.team_id as 'ID Team', team_name as 'Nama Tim', manager_name as 'Nama Manager', player_name as 'Nama Captain', concat(home_stadium, ', ', city) as 'home_stadium',city as 'city', capacity as 'capacity' FROM manager, team, player WHERE team.manager_id = manager.manager_id and player.player_id = team.captain_id";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtPlayerTM);
            ComboBoxTM.DataSource = dtPlayerTM;
            ComboBoxTM.DisplayMember = "Nama Tim";

            
            //copy mulai sini
            sqlQuery = "SELECT team.team_id as 'ID Team', team_name as 'Nama Tim', manager_name as 'Nama Manager', player_name as 'Nama Captain', concat(home_stadium, ', ', city) as 'home_stadium', city as 'city', capacity as 'capacity' FROM manager, team, player WHERE team.manager_id = manager.manager_id and player.player_id = team.captain_id";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtPlayerTR); // sampai sini
            ComboBoxTR.DataSource = dtPlayerTR;
            ComboBoxTR.DisplayMember = "Nama Tim";

            
        }

        private void ComboBoxTR_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxTR.ValueMember = "Nama Manager";
            LabelManagerTR.Text = ComboBoxTR.SelectedValue.ToString();
            ComboBoxTR.ValueMember = "Nama Captain";
            LabelCaptainTR.Text = ComboBoxTR.SelectedValue.ToString();
            ComboBoxTR.ValueMember = "capacity";
            LabelCapacity.Text = ComboBoxTR.SelectedValue.ToString();
            ComboBoxTR.ValueMember = "home_stadium";
            string Stadium = ComboBoxTR.SelectedValue.ToString();
            ComboBoxTR.ValueMember = "city";
            string City = ComboBoxTR.SelectedValue.ToString();
            LabelStadium.Text = Stadium + ", " + City;
        }

        private void ComboBoxTM_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxTM.ValueMember = "Nama Manager";
            LabelManagerTM.Text = ComboBoxTM.SelectedValue.ToString();
            ComboBoxTM.ValueMember = "Nama Captain";
            LabelCaptainTM.Text = ComboBoxTM.SelectedValue.ToString();
        }

        private void ButtonCheck_Click(object sender, EventArgs e)
        {
            LabelTanggal.Text = "";
            LabelSkor.Text = "";
            ComboBoxTR.ValueMember = "ID Team";
            ComboBoxTM.ValueMember = "ID Team";

            DataTable dataMatch = new DataTable();
            DataTable matchResult = new DataTable();
            sqlQuery = "select date_format(`match`.match_date, '%d %M %Y') as 'match_date', concat(`match`.goal_home,' - ', `match`.goal_away) as 'skor', d.match_id as 'match_id', d.`minute` as 'minute', d.team_id as 'team_id', d.player_id as 'player_id', d.`type` as 'type', d.`delete` as 'delete' from dmatch d, `match`, team t, team t1 where d.match_id =`match`.match_id and t.team_id =`match`.team_home and t1.team_id =`match`.team_away and `match`.team_home = '" + ComboBoxTR.SelectedValue.ToString() + "' and `match`.team_away = '" + ComboBoxTM.SelectedValue.ToString() + "'";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dataMatch);
            if (dataMatch.Rows.Count > 0)
            {
                LabelTanggal.Text = dataMatch.Rows[0]["match_date"].ToString();
                LabelSkor.Text = dataMatch.Rows[0]["skor"].ToString();
            }
            else
            {
                LabelTanggal.Text = "";
                LabelSkor.Text = "";
            }
            sqlQuery = "select d.`minute` as 'minute', if ((d.team_id = '" + ComboBoxTR.SelectedValue.ToString() + "' and d.type != 'GW') or(d.team_id = '" + ComboBoxTM.SelectedValue.ToString() + "' and d.type = 'GW') , p.player_name,' ') as 'Player Name 1', if ((d.team_id = '" + ComboBoxTR.SelectedValue.ToString() + "' and d.type != 'GW') or(d.team_id = '" + ComboBoxTM.SelectedValue.ToString() + "' and d.type = 'GW') , if (d.type = 'CY','YELLOW CARD',if (d.type = 'CR', 'RED CARD',if (d.type = 'GO', 'GOAL',if (d.type = 'GP','GOAL PENALTY',if (d.type = 'GW', 'OWN GOAL','PENALTY MISS'))))),' ') as 'Tipe 1', if ((d.team_id = '" + ComboBoxTM.SelectedValue.ToString() + "' and d.type != 'GW') OR(d.team_id = '" + ComboBoxTR.SelectedValue.ToString() + "' and d.type = 'GW'), p.player_name,' ') as 'Player Name 2', if ((d.team_id = '" + ComboBoxTM.SelectedValue.ToString() + "' and d.type != 'GW') OR(d.team_id = '" + ComboBoxTR.SelectedValue.ToString() + "' and d.type = 'GW'),if (d.type = 'CY','YELLOW CARD',if (d.type = 'CR', 'RED CARD',if (d.type = 'GO', 'GOAL',if (d.type = 'GP','GOAL PENALTY',if (d.type = 'GW', 'OWN GOAL','PENALTY MISS'))))),' ') as 'Type 2' from dmatch d, `match`, team t, team t1, player p where p.player_id = d.player_id and d.match_id =`match`.match_id and t.team_id =`match`.team_home and t1.team_id =`match`.team_away and `match`.team_home = '" + ComboBoxTR.SelectedValue.ToString() + "' and `match`.team_away = '" + ComboBoxTM.SelectedValue.ToString() + "'";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(matchResult);
            DataGridMatch.DataSource = matchResult;
        }
    }
}
