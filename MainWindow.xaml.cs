using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;

namespace ProjetExamen
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection DBConnection;
        public MainWindow()
        {
            InitializeComponent();

            //configuration de la chaine de connexion.
            DBConnection = new SqlConnection(@"Data Source=DESKTOP-2TDJ795\SQLEXPRESS;Initial Catalog=Stagiaire;Integrated Security=True");

            //initialisation des combobox à chaque démarrage de l'application.
            ChargerComboBox(cbxNomDuProgramme, "select nom from programmes");
            ChargerComboBox(cbxNumeroDuProgramme, "select numero from programmes");

        }

        /*.......................section programme.......................................*/
        /// <summary>
        /// Detecter le changement de text pour Reinitialiser  la couleur des bordures des TextBoxes apres la détection
        /// d'erreur(couleur rouge des bordure lors de la detection d'erreur)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void textChangedNumeroDuProgramme(object sender, TextChangedEventArgs args)
        {
            txtNumeroDuProgramme.ClearValue(Border.BorderBrushProperty);
        }
        private void textChangedNomDuProgramme(object sender, TextChangedEventArgs args)
        {
            txtNomDuProgramme.ClearValue(Border.BorderBrushProperty);
        }
        private void textChangedDureeDuProgramme(object sender, TextChangedEventArgs args)
        {
            txtDureeDuProgramme.ClearValue(Border.BorderBrushProperty);
        }


        private void btnAjouterProgramme_Click(object sender, RoutedEventArgs e)
        {

            insererProgramme();
            object[] champs = new object[] { txtNomDuProgramme, txtDureeDuProgramme, txtNumeroDuProgramme };
            EffacerChamps(champs);
            //Réinitialisation des combobox programmes à chaque ajout de programme.
            ChargerComboBox(cbxNomDuProgramme, "select nom from programmes");
            ChargerComboBox(cbxNumeroDuProgramme, "select numero from programmes");
        }

        private void btnSupprimerProgramme_Click(object sender, RoutedEventArgs e)
        {
            SupprimerProgramme();
        }

        private void btnEffacerProgramme_Click(object sender, RoutedEventArgs e)
        {
           
            object[] champs = new object[] { txtNomDuProgramme, txtDureeDuProgramme, txtNumeroDuProgramme};
            EffacerChamps(champs);
        }

        /*.......................Section Stagiaire.......................................*/
        private void textChangedNumeroDuStagiaire(object sender, TextChangedEventArgs args)
        {
            txtNumeroDuStagiaire.ClearValue(Border.BorderBrushProperty);
        }
        private void textChangedNomDuStagiaire(object sender, TextChangedEventArgs args)
        {
            txtNomDuStagiaire.ClearValue(Border.BorderBrushProperty);
        }
        private void textChangedPrenomDuStagiaire(object sender, TextChangedEventArgs args)
        {
            txtPrenomDuStagiaire.ClearValue(Border.BorderBrushProperty);
        }
        private void btnAjouterStagiaire_Click(object sender, RoutedEventArgs e)
        {
            if (insererStagiaire() == true)
            {
                //Rénitialisation des champs après chaque ajoute(on laisse les champs sexe et programme pour faciliter l'entrer des données).
                object[] champs = new object[] { txtNumeroDuStagiaire, txtNomDuStagiaire, txtPrenomDuStagiaire, DPdateDeNaissanceDuStagiaire };
                EffacerChamps(champs);
            }

        }

        private void btnSupprimerStagiaire_Click(object sender, RoutedEventArgs e)
        {
            supprimerStagiaire();
        }

        private void btnEffacerStagiaire_Click(object sender, RoutedEventArgs e)
        {
            object[] champs = new object[] { txtNumeroDuStagiaire, txtNomDuStagiaire, txtPrenomDuStagiaire, rbAutre, rbMasculin, rbFeminin, cbxNomDuProgramme, DPdateDeNaissanceDuStagiaire };
            EffacerChamps(champs);

        }

        /*----------------------Liste des stagiaires-------------------------------*/
        private void btnRechercher_Click(object sender, RoutedEventArgs e)
        {
            chargerListeStagiaires();
        }


        /*----------------------Les Fonctions urilisées-------------------------------*/

        /// <summary>
        /// pour verifier que les champs ne sont pas vides
        /// </summary>
        /// <param name="champs">Tableau qui contient tous les champs du formulaire</param>
        /// <returns>true si aucun champs n'est vide.</returns>
        private bool verifierRemplissageDesChamps(Object[] champs)
        {
            foreach (object champ in champs)
            {
                Type typeDuChamp = champ.GetType();
                if (typeDuChamp.Equals(typeof(TextBox)))
                {
                    TextBox textBox = (TextBox)champ;
                    if (textBox.Text == "")
                    {
                        textBox.BorderBrush = Brushes.Red;
                        textBox.Focus();
                        MessageBox.Show($"Veuillez remplir le champs \"{textBox.Name.Substring(3)}\"");
                        return false;
                    }
                }
                else if (typeDuChamp.Equals(typeof(ComboBox)))
                {
                    ComboBox comboBox = (ComboBox)champ;
                    if (comboBox.Text == "")
                    {
                        comboBox.BorderBrush = Brushes.Red;
                        comboBox.Focus();
                        MessageBox.Show($"Veuillez remplir le champs \"{comboBox.Name.Substring(3)}\"");
                        return false;
                    }
                }

                else if (typeDuChamp.Equals(typeof(DatePicker)))
                {
                    DatePicker selectioneurDedate = (DatePicker)champ;
                    if (selectioneurDedate.SelectedDate == null)
                    {
                        MessageBox.Show("Veuillez Choisir une date de naissance !");
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Pour verifier que l'utilisateur a entré une date.
        /// </summary>
        /// <param name="date">Un DatePicker</param>
        /// <returns>true si le Datepicker  n'est vide.</returns>
        private bool verifierRemplissangeDuSelecteurDeDates(DatePicker date)
        {

            if (date.Text == "")
            {
                date.BorderBrush = Brushes.Red;
                date.Focus();
                MessageBox.Show($"Veuillez choisir une date !");
                return false;

            }


            return true;
        }

        /// <summary>
        /// Vérifier si l'utilisateur a entré un entier.
        /// Cette méthode est utilisée pour vérifier les entrées de durée, et de numéro e programme, 
        /// avant de les inserer dans la base de données.
        /// </summary>
        /// <param name="champ">Le champ dont le contenu sera vérifié.</param>
        /// <returns>L'entier qui sera inserée par la suite dans la base de donnée</returns>
        private int verifierEntier(TextBox champ)
        {
            int champConverti = 0;
            try
            {
                champConverti = int.Parse(champ.Text);
                if (champConverti <= 0)
                {
                    champ.BorderBrush = Brushes.Red;
                    champ.Focus();
                    MessageBox.Show($"Veuillez entrer un {champ.Name.Substring(3)} valide");
                }
            }
            catch
            {
                champ.BorderBrush = Brushes.Red;
                champ.Focus();
                MessageBox.Show($"Veuillez entrer un {champ.Name.Substring(3)} valide");
            }

            return champConverti;


        }

        /// <summary>
        /// Vérifier si une chaine de charactère est conforme.
        /// Cette méthode est utilisée pour vérifier les entrée des nom, 
        /// et prénom avant de les inserer dans la base de données.
        /// </summary>
        /// <param name="champ">Le champ dont le contenu sera vérifié.</param>
        /// <returns>La chaine qui sera inserée dans la base de donnée</returns>
        private string verifierChaine(TextBox champ)
        {
            string resultat = champ.Text;
            //reEx pour vérification de nom et prénom, il supporte tous les caractères français.
            Regex reEx = new Regex(@"^[a-z A-Z àÀâçÇéÉÈèêÂÊ-]+$");
            if (!reEx.IsMatch(resultat))
            {
                champ.BorderBrush = Brushes.Red;
                champ.Focus();
                MessageBox.Show($"Veuillez Entrer un {champ.Name.Substring(3, champ.Name.IndexOf('m') - 2)} valide !");
                resultat = "";
            }
            return resultat;
        }

        /// <summary>
        /// Vérifier si la case du sexe a été cocher.
        /// </summary>
        /// <returns></returns>
        private string verifierSexe()
        {
            string sexe = "";
            if (rbMasculin.IsChecked == true)
            {
                sexe = "Masculin";
            }
            else if (rbFeminin.IsChecked == true)
            {
                sexe = "Feminin";
            }
            else if (rbAutre.IsChecked == true)
            {
                sexe = "Autre";
            }
            else
            {
                MessageBox.Show("Il faut choisir le sexe du stagiaire !");

            }
            return sexe;
        }

        /// <summary>
        /// Remplir un comboBox avec les données des programme(form "stagiaire" avec les nom des programme, form "liste des stagiaires" avec les num des programmes).
        /// </summary>
        private void ChargerComboBox(ComboBox cbx, string requete)
        {

            SqlCommand commande = new SqlCommand(requete, DBConnection);
            //ouvrir la connexion.
            DBConnection.Open();
            SqlDataReader datareader = commande.ExecuteReader();
            while (datareader.Read())
            {
                cbx.Items.Add(datareader[0]);
            }
            //ferner la connexion.
            DBConnection.Close();
        }

        /// <summary>
        /// Inserer un nouveau programme à la base de donée
        /// </summary>
        private void insererProgramme()
        {
            TextBox[] champs = new TextBox[] { txtNumeroDuProgramme, txtNomDuProgramme, txtDureeDuProgramme };
            if (verifierRemplissageDesChamps(champs) == true)
            {
                int numero = verifierEntier(txtNumeroDuProgramme);
                string nom = verifierChaine(txtNomDuProgramme);
                int duree = verifierEntier(txtDureeDuProgramme);
                if (numero > 0 && nom != "" && duree > 0)
                {
                    //Requet pour l'insertion d'un programme dans la base de donnee.
                    string requete = $"insert into Programmes values(@nom,@duree,@numero)";
                    //Ma commande.
                    SqlCommand commande = new SqlCommand(requete, DBConnection);
                    //Comment executer ma requete.
                    commande.CommandType = CommandType.Text;
                    //Recuperer les informations a mettre dans les prametres.
                    commande.Parameters.AddWithValue("@nom", nom);
                    commande.Parameters.AddWithValue("@duree", duree);
                    commande.Parameters.AddWithValue("@numero", numero);
                    //ouvrir la connexion.
                    DBConnection.Open();
                    commande.ExecuteNonQuery();
                    //fermer la connexion.
                    DBConnection.Close();
                    MessageBox.Show("Le programme a été ajouté avec succès !");
                }

            }
        }

        /// <summary>
        /// Supprimer un programme de la base de donée
        /// </summary>
        private void SupprimerProgramme()
        {
            TextBox[] champs = new TextBox[] { txtNumeroDuProgramme, txtNomDuProgramme };
            if (verifierRemplissageDesChamps(champs) == true)
            {
                int numero = verifierEntier(txtNumeroDuProgramme);
                string nom = verifierChaine(txtNomDuProgramme);

                if (numero > 0 && nom != "")
                {
                    try
                    {
                        //Requet pour l'insertion d'un programme dans la base de donnee.
                        string requete = $"delete from Programmes where nom = @nom and numero = @numero";
                        //Ma commande.
                        SqlCommand commande = new SqlCommand(requete, DBConnection);
                        //Comment executer ma requete.
                        commande.CommandType = CommandType.Text;
                        //Recuperer les informations a mettre dans les prametres.
                        commande.Parameters.AddWithValue("@nom", nom);
                        commande.Parameters.AddWithValue("@numero", numero);
                        //ouvrir la connexion.
                        DBConnection.Open();
                        //executer la requette.
                        commande.ExecuteNonQuery();
                        //fermer la connexion.
                        DBConnection.Close();
                        MessageBox.Show("Le programme a été supprimé avec avec succès !");
                    }
                    catch
                    {
                        MessageBox.Show("Le programme recherché n'existe pas !");
                    }
                }

            }
        }

        /// <summary>
        /// Inserer un nouveau stagiaire à la base de donée
        /// </summary>
        private bool insererStagiaire()
        {
            Object[] champs = new Object[] { txtNumeroDuStagiaire, txtNomDuStagiaire, txtPrenomDuStagiaire, cbxNomDuProgramme, DPdateDeNaissanceDuStagiaire };


            if (verifierRemplissageDesChamps(champs) == true)
            {

                int numero = verifierEntier(txtNumeroDuStagiaire);
                string nom = verifierChaine(txtNomDuStagiaire);
                string prenom = verifierChaine(txtPrenomDuStagiaire);
                string sexe = verifierSexe();
                verifierRemplissangeDuSelecteurDeDates(DPdateDeNaissanceDuStagiaire);


                if (sexe != "" && prenom != "" && nom != "" && numero > 0 && DPdateDeNaissanceDuStagiaire.SelectedDate.Value != null && cbxNomDuProgramme.SelectedItem != null)
                {
                    DateTime dateDeNaissance = DPdateDeNaissanceDuStagiaire.SelectedDate.Value;
                    string nomDuProgramme = cbxNomDuProgramme.SelectedItem.ToString();
                    int idProgramme = getIdProgramme(nomDuProgramme);
                    //Requet sql pour l'insertion d'un nouveau stagiaire dans la base de données.
                    string requete = $"insert into Etudiant values(@numero,@nom,@prenom, @date_de_naissance, @sexe, @id_programme)";
                    //Ma commande.
                    SqlCommand commande = new SqlCommand(requete, DBConnection);
                    //Comment executer ma requete.
                    commande.CommandType = CommandType.Text;
                    //Recuperer les informations a mettre dans les prametres.
                    commande.Parameters.AddWithValue("@nom", nom);
                    commande.Parameters.AddWithValue("@prenom", prenom);
                    commande.Parameters.AddWithValue("@numero", numero);
                    commande.Parameters.AddWithValue("@sexe", sexe);
                    commande.Parameters.AddWithValue("@date_de_naissance", dateDeNaissance);
                    commande.Parameters.AddWithValue("@id_programme", idProgramme);
                    //ouvrir la connexion.
                    DBConnection.Open();
                    //executer la requette.
                    commande.ExecuteNonQuery();
                    //fermer la connexion.
                    DBConnection.Close();
                    MessageBox.Show("Le stagiaire a été ajouté avec succès !");
                    return true;
                }
            }
            return false;
        }

    

        /// <summary>
        /// supprimer un stagiaire de la base de donée
        /// </summary>
        private void supprimerStagiaire()
        {
            TextBox[] champs = new TextBox[] { txtNumeroDuStagiaire, txtNomDuStagiaire, txtPrenomDuStagiaire };
            if (verifierRemplissageDesChamps(champs) == true)
            {
                try
                {
                    int numero = verifierEntier(txtNumeroDuStagiaire);
                    string nom = verifierChaine(txtNomDuStagiaire);
                    string prenom = verifierChaine(txtPrenomDuStagiaire);

                    if (prenom != "" && nom != "" && numero > 0)
                    {
                        //Requet sql pour l'insertion d'un nouveau stagiaire dans la base de données.
                        string requete = $"delete from Etudiant where nom = @nom and prenom = @prenom and numero = @numero";
                        //Ma commande.
                        SqlCommand commande = new SqlCommand(requete, DBConnection);
                        //Comment executer ma requete.
                        commande.CommandType = CommandType.Text;
                        //Recuperer les informations a mettre dans les prametres.
                        commande.Parameters.AddWithValue("@nom", nom);
                        commande.Parameters.AddWithValue("@prenom", prenom);
                        commande.Parameters.AddWithValue("@numero", numero);
                        //ouvrir la connexion.
                        DBConnection.Open();
                        //executer la requette.
                        commande.ExecuteNonQuery();
                        //fermer la connexion.
                        DBConnection.Close();

                        MessageBox.Show("Le stagiaire a été supprimé avec avec succès !");
                    }

                }
                catch
                {
                    MessageBox.Show("Le Stagiaire recherché n'existe pas !");
                }
            }


        }

        /// <summary>
        /// Methode pour récuperer le id_programme a partir du nom du programme.
        /// </summary>
        /// <param name="nomDuProgramme">nom du programme</param>
        /// <returns>int qui représente le id_programme.</returns>
        private int getIdProgramme(string nomDuProgramme)
        {
            string requete = $"select id_programme from Programmes where nom = @nom";
            SqlCommand commande = new SqlCommand(requete, DBConnection);
            commande.Parameters.AddWithValue("@nom", nomDuProgramme);
            //ouvrir la connexion.
            DBConnection.Open();
            SqlDataReader datareader = commande.ExecuteReader();
            datareader.Read();
            try
            {
                int idProgramme = (int)datareader[0];
                DBConnection.Close();
                return idProgramme;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return 0;
            }
        }

        /// <summary>
        /// Methode pour récuperer le id_programme a partir du numéro de programme.
        /// </summary>
        /// <param name="nomDuProgramme">nom du programme</param>
        /// <returns>int qui représente le id_programme.</returns>
        private int getIdProgramme(int numeroDuProgramme)
        {
            string requete = $"select id_programme from Programmes where numero = @numero";
            SqlCommand commande = new SqlCommand(requete, DBConnection);
            commande.Parameters.AddWithValue("@numero", numeroDuProgramme);
            //ouvrir la connexion.
            DBConnection.Open();
            SqlDataReader datareader = commande.ExecuteReader();

            try
            {
                int idProgramme = 0;
                while (datareader.Read())
                {

                    idProgramme = (int)datareader[0];
                }

                DBConnection.Close();
                return idProgramme;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return 0;
            }


        }

        /// <summary>
        /// Methode pour récuperer le id_programme a partir du numéro de programme.
        /// </summary>
        /// <param name="nomDuProgramme">nom du programme</param>
        /// <returns>int qui représente le id_programme.</returns>
        private string getNomDuProgramme(int idProgramme)
        {
            string requete = $"select nom from Programmes where id_programme = @id_programme";
            SqlCommand commande = new SqlCommand(requete, DBConnection);
            commande.Parameters.AddWithValue("@id_programme", idProgramme);
            //ouvrir la connexion.


            DBConnection.Open();
            SqlDataReader datareader = commande.ExecuteReader();
            try
            {
                string nomDuProgramme = "";
                datareader.Read();
                nomDuProgramme = (string)datareader[0];
                DBConnection.Close();
                return nomDuProgramme;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return "";
            }
        }

        /// <summary>
        /// Reinitialise Les champs passer en parametres 
        /// </summary>
        /// <param name="champs">Tableau de chmaps</param>
        private void EffacerChamps(object[] champs)
        {
            foreach (object champ in champs)
            {
                Type typeDuChamp = champ.GetType();
                if (typeDuChamp.Equals(typeof(TextBox)))
                {
                    TextBox textBox = (TextBox)champ;
                    textBox.Text = "";
                }
                else if (typeDuChamp.Equals(typeof(ComboBox)))
                {
                    ComboBox comboBox = (ComboBox)champ;
                    comboBox.SelectedIndex = -1;
                }
                else if (typeDuChamp.Equals(typeof(RadioButton)))
                {
                    RadioButton boutonRadio = (RadioButton)champ;
                    boutonRadio.IsChecked = false;
                }
                else if (typeDuChamp.Equals(typeof(DatePicker)))
                {
                    DatePicker selectioneurDedate = (DatePicker)champ;
                    selectioneurDedate.SelectedDate = null;
                }
            }
        }

        /// <summary>
        /// Remplir la listes des stagiaires.
        /// </summary>
        private void chargerListeStagiaires()
        {
            try
            {
                int idProgramme = getIdProgramme(int.Parse(cbxNumeroDuProgramme.Text));
                string nomDuprogramme = getNomDuProgramme(idProgramme);
                string requete = "select numero, nom, prenom, date_de_naissance, sexe  from Etudiant where id_programme = @id_programme ";
                SqlCommand commande = new SqlCommand(requete, DBConnection);
                commande.Parameters.AddWithValue("@id_programme", idProgramme);
                //ouvrir la connexion.
                DBConnection.Open();
                SqlDataReader datareader = commande.ExecuteReader();
                List<Stagiaire> stagiaires = new List<Stagiaire>();
                while (datareader.Read())
                {
                    Stagiaire stagiaire = new Stagiaire((int)datareader[0], (string)datareader[1], (string)datareader[2], (DateTime)datareader[3], (string)datareader[4], nomDuprogramme);
                    stagiaires.Add(stagiaire);
                }

                DataGridListeDesStagiares.ItemsSource = stagiaires;
                DBConnection.Close();
            }
            catch
            {
                MessageBox.Show("Veuillez Selectionner un programme !");
            }

        }
    }
}

