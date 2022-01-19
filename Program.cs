using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace DM_BDD_ARISS_ATALLAH
{
    public class Program
    {
        static void Main(string[] args)
        {
            MySqlConnection maConnexion = null;
            try
            {
                string connexionString = "SERVER=localhost;PORT=3306;" +
                                         "DATABASE=cooking;" +
                                         "UID=root;PASSWORD=Djnmpdc12";

                maConnexion = new MySqlConnection(connexionString);

            }
            catch (MySqlException e)
            {
                Console.WriteLine(" ErreurConnexion : " + e.ToString());
                return;
            }
            
            int SaisieNombre()
            {
                int nombre;
                while (!int.TryParse(Console.ReadLine(), out nombre))
                {
                    Console.WriteLine("Erreur ! Veuillez rentrez un entier : ");
                }
                return nombre;
            }
            Commande_Fournisseur(maConnexion);
            Peremption(maConnexion);
            ConsoleKeyInfo cki;
            //Programme principal
            do
            {
                Console.Clear();
                Console.WriteLine("************** BIENVENUE SUR COOKING **************\n");
                Console.WriteLine("Menu :\n\n"
                                 + "Option 1 : Espace Client\n"
                                 + "Option 2 : Espace CréateurDeRecette\n"
                                 + "Option 3 : Classements\n"
                                 + "Option 4 : Mode DEMO\n\n"
                                 + "Sélectionnez l'option désiré : ");
                int option = SaisieNombre();
                switch (option)
                {
                    case 1:
                        Commander(maConnexion, false);
                        break;

                    case 2:

                        Console.Clear();
                        Console.WriteLine("Espace CréateurDeRecette :\n\n"
                                 + "Option 1 : Effectuer une commande\n"
                                 + "Option 2 : Créer une recette\n"
                                 + "Option 3 : Consulter votre solde\n"
                                 + "Option 4 : Consulter vos recettes\n"
                                 + "Option 5 : Supprimer une recette\n"
                                 + "Option 6 : Suppression de compte\n\n"
                                 + "Sélectionnez l'option désiré : ");
                        int sousOption = SaisieNombre();
                        switch (sousOption)
                        {
                            //Sous-options
                            case 1:
                                Commander(maConnexion, true);
                                break;

                            case 2:
                                Ajout_Recette(maConnexion);
                                break;

                            case 3:
                                string num = Identification(maConnexion, true);
                                maConnexion.Open();
                                string sql = "SELECT points_Cook FROM CréateurDeRecettes WHERE Num_CdR='" + num + "';";
                                MySqlCommand command1 = maConnexion.CreateCommand();
                                command1.CommandText = sql;
                                MySqlDataReader reader = command1.ExecuteReader();
                                int solde = 0;
                                while (reader.Read())
                                {
                                    solde = reader.GetInt32(0);
                                }
                                Console.Clear();
                                Console.WriteLine("Votre solde est de : " + solde+" points Cook");
                                maConnexion.Close();
                                break;

                            case 4:
                                string numero = Identification(maConnexion, true);
                                Console.Clear();
                                string req = "SELECT  Nom_Recette,count(*) FROM Recette JOIN historique_client ON Recette_Commandée=Nom_Recette WHERE ID_CdR='"+numero+"' " +
                                    "GROUP BY Nom_Recette;" ;
                                MySqlCommand command = maConnexion.CreateCommand();
                                command.CommandText = req;
                                maConnexion.Open();
                                MySqlDataReader readerbis = command.ExecuteReader();
                                Console.WriteLine("Statistiques des commandes de vos recettes : ");
                                while (readerbis.Read())
                                {
                                    Console.WriteLine(readerbis.GetValue(0) + " -> Quantité : " + readerbis.GetInt32(1));
                                }
                                maConnexion.Close();
                                break;

                            case 5:
                                num = Identification(maConnexion, true);
                                Supprimer_Recette(maConnexion, num);
                                break;

                            case 6:
                                num = Identification(maConnexion, true);
                                Console.WriteLine("Voulez-vous supprimer votre compte ? (vous resterez néanmoins client ordinaire) oui/non ");
                                if (Console.ReadLine() == "oui")
                                { Supprimer_Recette(maConnexion, num, true); Supprimer_CdR(maConnexion, num); }
                                else
                                {
                                    Console.WriteLine("Retour vers le menu principal");
                                }
                                break;

                        }
                        break;

                    case 3:

                        Console.Clear();
                        Console.WriteLine("Classements COOKING :\n\n"
                                 + "Option 1 : CdR de la semaine\n"
                                 + "Option 2 : Top 5 recettes les plus commandées\n"
                                 + "Option 3 : Catégorie CdR d'OR\n\n"
                                 + "Sélectionnez l'option désiré : ");
                        int sousOption2 = SaisieNombre();
                        switch (sousOption2)
                        {
                            //Sous-options
                            case 1:
                                Console.WriteLine("-----------------------------------------CdR de la semaine------------------------------------------\n");

                                Classement_Hebdomadaire(maConnexion);
                                break;

                            case 2:
                                Console.WriteLine("-----------------------------------------Top 5 recettes les plus commandées---------------------------------------\n");

                                Top5_Recette(maConnexion);
                                break;

                            case 3:
                                Console.WriteLine("------------------------------------------Catégorie CdR d'OR---------------------------------------------------\n");
                                CdR_Or(maConnexion);
                                break;

                        }
                        break;

                    case 4:
                        Console.Clear();
                        Console.WriteLine("Mode DEMO :\n\n"
                                 + "Option 1 : Informations clients\n"
                                 + "Option 2 : Informations à CdR\n"
                                 + "Option 3 : Informations recettes\n"
                                 + "Option 4 : Informations produits\n\n"
                                 + "Sélectionnez l'option désiré : ");
                        sousOption2 = SaisieNombre();
                        switch (sousOption2)
                        {
                            //Sous-options
                            case 1:
                                Console.Clear();
                                Console.WriteLine("-----------------------------------------Nombre de Clients------------------------------------------\n");
                                string requete = "SELECT count(*) FROM client;";
                                MySqlCommand command = maConnexion.CreateCommand();
                                command.CommandText = requete;
                                MySqlDataReader readerbis = command.ExecuteReader();
                                Console.Write("Il y a : ");
                                while (readerbis.Read())
                                {
                                    Console.WriteLine(readerbis.GetValue(0) + " clients");
                                }
                                readerbis.Close();

                                break;

                            case 2:
                                Console.Clear();

                                Console.WriteLine("-----------------------------------------Informations CDR---------------------------------------\n");
                                requete = "SELECT Nom_CdR,count(*) FROM créateurderecettes JOIN Recette on ID_CdR=Num_CdR " +
                                    "JOIN historique_client on Recette_Commandée = Nom_Recette group by Nom_CdR; ";
                                MySqlCommand command2 = maConnexion.CreateCommand();

                                command2.CommandText = requete;
                                readerbis = command2.ExecuteReader();
                                Console.WriteLine("Voici la liste des CdR : ");
                                while (readerbis.Read())
                                {
                                    Console.WriteLine("Les recettes de " + readerbis.GetValue(0) + " ont été commandées " + readerbis.GetValue(1) + " fois");
                                }
                                readerbis.Close();
                                string req = "SELECT Nom_CdR,0 FROM créateurderecettes where Nom_CdR not in (SELECT Nom_CdR FROM créateurderecettes JOIN Recette on ID_CdR=Num_CdR " +
                                    "JOIN historique_client on Recette_Commandée = Nom_Recette group by Nom_CdR); ";
                                command2.CommandText = req;
                                readerbis = command2.ExecuteReader();
                                while (readerbis.Read())
                                {
                                    Console.WriteLine("Les recettes de " + readerbis.GetValue(0) + " ont été commandées " + readerbis.GetValue(1) + " fois");

                                }
                                readerbis.Close();
                                break;

                            case 3:
                                Console.Clear();

                                Console.WriteLine("------------------------------------------Nombre de Recettes---------------------------------------------------\n");
                                requete = "SELECT count(*) FROM recette;";
                                MySqlCommand command3 = maConnexion.CreateCommand();
                                command3.CommandText = requete;
                                readerbis = command3.ExecuteReader();
                                Console.Write("Il y a : ");
                                while (readerbis.Read())
                                {
                                    Console.WriteLine(readerbis.GetValue(0) + " recettes");
                                }
                                readerbis.Close();
                                break;
                            case 4:
                                Console.Clear();

                                Console.WriteLine("------------------------------------------Informations Produits---------------------------------------------------\n");
                                requete = "SELECT Nom_Prod FROM produit WHERE stock_actuel<=2*stock_min;";
                                MySqlCommand command4 = maConnexion.CreateCommand();
                                command4.CommandText = requete;
                                readerbis = command4.ExecuteReader();
                                Console.WriteLine("Les produits concernés sont  : ");
                                while (readerbis.Read())
                                {
                                    Console.WriteLine(readerbis.GetValue(0) );
                                }
                                readerbis.Close();
                            
                                Console.WriteLine("slectionnez un produit de la liste précédente : ");
                                string nom = Console.ReadLine();
                                requete = "SELECT categorie,unité FROM produit WHERE Nom_Prod='"+nom+"';";
                                command4 = maConnexion.CreateCommand();
                                command4.CommandText = requete;
                                readerbis = command4.ExecuteReader();
                                Console.WriteLine("Les recettes concernés par ce produit sont  : ");
                                while (readerbis.Read())
                                {
                                    Console.WriteLine(readerbis.GetValue(0)+ " composé de "+readerbis.GetValue(1)+" unités");
                                }
                                readerbis.Close();
                                break;
                        }

                        break;
                    default:
                        break;

                }

                Console.WriteLine("\nTapez ESCAPE pour sortir ou taper ENTRER pour selectionner une autre option du menu");
                cki = Console.ReadKey();
            } while (cki.Key != ConsoleKey.Escape);
            
            

            Console.ReadKey();
        }
        #region liste des methodes permettant l'ajout des différents corps
        static void Ajout_Fournisseur(MySqlConnection maConnexion, string num, string nom)
        {
            maConnexion.Open();

            string sql = "Insert into Fournisseur (Num_Fournisseur, Nom_Fournisseur) "
                                                + " values (@Num_Fournisseur, @Nom_Fournisseur) ";



            MySqlCommand command4 = maConnexion.CreateCommand();
            command4.CommandText = sql;

            MySqlParameter Num_Fournisseur = new MySqlParameter("@Num_Fournisseur", MySqlDbType.VarChar);
            Num_Fournisseur.Value = num;
            command4.Parameters.Add(Num_Fournisseur);

            MySqlParameter Nom_Fournisseur = new MySqlParameter("@Nom_Fournisseur", MySqlDbType.VarChar);
            Nom_Fournisseur.Value = nom;
            command4.Parameters.Add(Nom_Fournisseur);



            int rowCount = command4.ExecuteNonQuery();


            maConnexion.Close();
        }
        static void Ajout_Produit(MySqlConnection maConnexion, string nom, string cat, int u, int sa, int smin, int smax,string ref_fournisseur)
        {
            maConnexion.Open();

            string sql = "Insert into Produit (Nom_Prod, categorie, unité,stock_actuel,stock_min,stock_max,reference) "
                                                + " values (@Nom_Prod, @categorie, @unité,@stock_actuel,@stock_min,@stock_max,@reference) ";



            MySqlCommand command4 = maConnexion.CreateCommand();
            command4.CommandText = sql;

            MySqlParameter Nom_Prod = new MySqlParameter("@Nom_Prod", MySqlDbType.VarChar);
            Nom_Prod.Value = nom;
            command4.Parameters.Add(Nom_Prod);

            MySqlParameter categorie = new MySqlParameter("@categorie", MySqlDbType.VarChar);
            categorie.Value = cat;
            command4.Parameters.Add(categorie);

            MySqlParameter unité = new MySqlParameter("@unité", MySqlDbType.Int32);
            unité.Value = u;
            command4.Parameters.Add(unité);

            MySqlParameter stock_actuel = new MySqlParameter("@stock_actuel", MySqlDbType.Int32);
            stock_actuel.Value = sa;
            command4.Parameters.Add(stock_actuel);

            MySqlParameter stock_min = new MySqlParameter("@stock_min", MySqlDbType.Int32);
            stock_min.Value = smin;
            command4.Parameters.Add(stock_min);

            MySqlParameter stock_max = new MySqlParameter("@stock_max", MySqlDbType.Int32);
            stock_max.Value = smax;
            command4.Parameters.Add(stock_max);

            MySqlParameter reference = new MySqlParameter("@reference", MySqlDbType.VarChar);
            reference.Value = ref_fournisseur;
            command4.Parameters.Add(reference);
            try
            {
                int rowCount = command4.ExecuteNonQuery();
            }
            catch(Exception e) {  }

            maConnexion.Close();
        }
        static void Ajout_Cuisinier(MySqlConnection maConnexion, string id, float salaire)
        {
            maConnexion.Open();

            string sql = "Insert into Cuisinier (ID_Cuisinier, Salaire) "
                                                + " values (@ID_Cuisinier, @Salaire) ";



            MySqlCommand command4 = maConnexion.CreateCommand();
            command4.CommandText = sql;

            MySqlParameter ID_Cuisinier = new MySqlParameter("@ID_Cuisinier", MySqlDbType.VarChar);
            ID_Cuisinier.Value = id;
            command4.Parameters.Add(ID_Cuisinier);

            MySqlParameter Salaire = new MySqlParameter("@Salaire", MySqlDbType.Float);
            Salaire.Value = salaire;
            command4.Parameters.Add(Salaire);



            int rowCount = command4.ExecuteNonQuery();


            maConnexion.Close();
        }
        static void Ajout_CdR(MySqlConnection maConnexion, string num, string nom, string adresse, string mail, int points)
        {
            maConnexion.Open();

            string sql = "Insert into CréateurDeRecettes (Num_CdR, Nom_CdR, adresse_CdR,email_CdR,points_Cook) "
                                                + " values (@Num_CdR, @Nom_CdR, @adresse_CdR,@email_CdR,@points_Cook) ";



            MySqlCommand command4 = maConnexion.CreateCommand();
            command4.CommandText = sql;

            MySqlParameter Num_CdR = new MySqlParameter("@Num_CdR", MySqlDbType.VarChar);
            Num_CdR.Value = num;
            command4.Parameters.Add(Num_CdR);

            MySqlParameter Nom_CdR = new MySqlParameter("@Nom_CdR", MySqlDbType.VarChar);
            Nom_CdR.Value = nom;
            command4.Parameters.Add(Nom_CdR);

            MySqlParameter adresse_CdR = new MySqlParameter("@adresse_CdR", MySqlDbType.VarChar);
            adresse_CdR.Value = adresse;
            command4.Parameters.Add(adresse_CdR);

            MySqlParameter points_Cook = new MySqlParameter("@points_Cook", MySqlDbType.Int32);
            points_Cook.Value = points;
            command4.Parameters.Add(points_Cook);

            MySqlParameter email_CdR = new MySqlParameter("@email_CdR", MySqlDbType.VarChar);
            email_CdR.Value = mail.ToLower();
            command4.Parameters.Add(email_CdR);

            int rowCount = command4.ExecuteNonQuery();


            maConnexion.Close();
            Ajout_Client(maConnexion, nom, adresse, num, mail);
        }
        static void Ajout_Recette(MySqlConnection maConnexion)
        {
            string numcdr = Identification(maConnexion, true);
            maConnexion.Open();
            Console.WriteLine("----------------------------------------- Espace création recette ------------------------------------------\n");

            Console.WriteLine("Nom de la Recette :");
            string nom = Console.ReadLine();
            Console.WriteLine("Type de la Recette :");
            string type_R = Console.ReadLine();
            Console.WriteLine("Description : ");
            string description = Console.ReadLine();
            float price = 0;
            bool p = false;

            while (p != true)
            {
                Console.WriteLine("Prix fixé (entre 10 et 40 cook) : ");
                price = (float)Convert.ToDouble(Console.ReadLine());
                if (price >= 10 && price <= 40)
                {
                    p = true;
                }

            }
            string sql = "Insert into Recette (Nom_Recette, type, descriptif,prix,ID_CdR) "
                                                + " values (@Nom_Recette, @type, @descriptif,@prix,@ID_CdR) ";



            MySqlCommand command4 = maConnexion.CreateCommand();
            command4.CommandText = sql;

            MySqlParameter Nom_Recette = new MySqlParameter("@Nom_Recette", MySqlDbType.VarChar);
            Nom_Recette.Value = nom;
            command4.Parameters.Add(Nom_Recette);

            MySqlParameter type = new MySqlParameter("@type", MySqlDbType.VarChar);
            type.Value = type_R;
            command4.Parameters.Add(type);

            MySqlParameter descriptif = new MySqlParameter("@descriptif", MySqlDbType.VarChar);
            descriptif.Value = description;
            command4.Parameters.Add(descriptif);

            MySqlParameter prix = new MySqlParameter("@prix", MySqlDbType.Float);
            prix.Value = price;
            command4.Parameters.Add(prix);

            MySqlParameter ID_CdR = new MySqlParameter("@ID_CdR", MySqlDbType.VarChar);
            ID_CdR.Value = numcdr;
            command4.Parameters.Add(ID_CdR);
            try
            {
                int rowCount = command4.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Console.WriteLine("Cette recette existe déjà");
            }
            Console.WriteLine("De combien de produit est constitué la recette ?");
            int nb = Convert.ToInt32(Console.ReadLine());

            //Requetes Fournisseur
            string sql_founisseur = "";
            string sql_ref = "";


            while (nb != 0)
            {
                Console.WriteLine("Quels en sont les produits ?");
                Console.Write("Saisissez le nom du produit : ");
                //PRoduits associés
                string nom_produit = Console.ReadLine();
                
                Console.Write("Saisissez l'unité du produit : ");
                int unite = Convert.ToInt32(Console.ReadLine());
                //Fournisseur associé
                //maConnexion.Open();

                sql_founisseur = "SELECT Nom_Fournisseur from Fournisseur;";
                MySqlCommand command_Fournisseur = maConnexion.CreateCommand();
                command_Fournisseur.CommandText = sql_founisseur;
                MySqlDataReader reader_Fournisseur = command_Fournisseur.ExecuteReader();
                string Nom = "";
                while (reader_Fournisseur.Read())// parcours ligne par ligne
                {
                    Nom = reader_Fournisseur.GetString(0); //récupération de la 1ère colonne

                    Console.WriteLine("Catégorie Founisseur :" + Nom );

                }
                reader_Fournisseur.Close();
                

                Console.WriteLine("\nSélectionnez une catégorie fournisseur associée : ");
                string fournisseur = Console.ReadLine();

                sql_ref = "SELECT Num_Fournisseur FROM Fournisseur WHERE Nom_Fournisseur='" + fournisseur + "';";
                MySqlCommand command2 = maConnexion.CreateCommand();
                command2.CommandText = sql_ref;
                MySqlDataReader reader2 = command2.ExecuteReader();
                string Num = "";
                while(reader2.Read())
                {
                    Num = reader2.GetString(0);
                }
                reader2.Close();
                    



                maConnexion.Close();

                Ajout_Produit(maConnexion, nom_produit, nom, unite, 90, 10, 100,Num);
                maConnexion.Open();
                string requete = "SELECT stock_actuel from Produit where Nom_Prod ='" + nom_produit + "';";

                MySqlCommand command1 = maConnexion.CreateCommand();
                command1.CommandText = requete;
                MySqlDataReader reader = command1.ExecuteReader();
                int stock = 0;
                while (reader.Read())// parcours ligne par ligne
                {
                    stock = reader.GetInt32(0);
                }



                reader.Close();
                maConnexion.Close();

                nb--;
            }

            maConnexion.Open();
            string sql2 = "update  CréateurDeRecettes,Recette set points_Cook = @points_Cook where  Num_CdR='" + numcdr + "';";
            MySqlCommand command5 = maConnexion.CreateCommand();

            MySqlParameter points_Cook = new MySqlParameter("@points_Cook", MySqlDbType.Int32);
            command5.CommandText = sql2;

            string requetepoint = "SELECT points_Cook FROM CréateurDeRecettes  where Num_CdR='" + numcdr + "';";
            MySqlCommand commandPoint = maConnexion.CreateCommand();
            commandPoint.CommandText = requetepoint;
            MySqlDataReader readerpoint = commandPoint.ExecuteReader();
            int pts = 0;

            while (readerpoint.Read())
            {
                pts = readerpoint.GetInt32(0);
            }
            readerpoint.Close();
            pts += 2;   //rémuneration de 2 cook pour chaque  recette créée

            points_Cook.Value = pts;

            command5.Parameters.Add(points_Cook);

            command5.ExecuteNonQuery();


            maConnexion.Close();
        }
        static void Ajout_Client(MySqlConnection maConnexion, string nom, string adresseC, string num, string mail)
        {
            maConnexion.Open();

            string sql = "Insert into CLient (Num_Client, Nom_Client, adresse,email) "
                                                + " values (@Num_Client, @Nom_Client, @adresse,@email) ";



            MySqlCommand command4 = maConnexion.CreateCommand();
            command4.CommandText = sql;

            MySqlParameter Nom_Client = new MySqlParameter("@Nom_Client", MySqlDbType.VarChar);
            Nom_Client.Value = nom;
            command4.Parameters.Add(Nom_Client);

            MySqlParameter adresse = new MySqlParameter("@adresse", MySqlDbType.VarChar);
            adresse.Value = adresseC;
            command4.Parameters.Add(adresse);

            MySqlParameter Num_Client = new MySqlParameter("@Num_Client", MySqlDbType.VarChar);
            Num_Client.Value = num;
            command4.Parameters.Add(Num_Client);

            MySqlParameter email = new MySqlParameter("@email", MySqlDbType.VarChar);
            email.Value = mail.ToLower();
            command4.Parameters.Add(email);

            int rowCount = command4.ExecuteNonQuery();


            maConnexion.Close();
        }


        #endregion
        /// <summary>
        /// Ajout d'une commande dans la BDD
        /// </summary>
        /// <param name="maConnexion"></param>
        /// <param name="date"></date de la commande>
        /// <param name="num"></numéro client>
        /// <param name="recette"></nom dela recette commandée>
        static void Historique_Commande(MySqlConnection maConnexion, DateTime date, string num, string recette)
        {
            //maConnexion.Open();

            string sql = "Insert into Historique_Client (Date_Commande, ID_Client, Recette_Commandée) "
                                               + " values (@Date_Commande, @ID_Client, @Recette_Commandée) ";

            MySqlCommand command4 = maConnexion.CreateCommand();
            command4.CommandText = sql;

            MySqlParameter Date_Commande = new MySqlParameter("@Date_Commande", MySqlDbType.DateTime);

            Date_Commande.Value = DateTime.Now;
            command4.Parameters.Add(Date_Commande);

            MySqlParameter ID_Client = new MySqlParameter("@ID_Client", MySqlDbType.VarChar);
            ID_Client.Value = num;
            command4.Parameters.Add(ID_Client);


            MySqlParameter Recette_Commandée = new MySqlParameter("@Recette_Commandée", MySqlDbType.VarChar);
            Recette_Commandée.Value = recette;
            command4.Parameters.Add(Recette_Commandée);
            try
            {
                command4.ExecuteNonQuery();
            }
            catch (Exception e) { }

            maConnexion.Close();
        }
        /// <summary>
        /// fonction permettant de s'identifier en tant que membre
        /// </summary>
        /// <param name="maConnexion"></param>
        /// <param name="cdr"></si le membre qui se connecte est un CdR alors c'est true>
        /// <returns></returns>
        static string Identification(MySqlConnection maConnexion, bool cdr)
        {
            try { maConnexion.Open(); }
            catch(Exception e) { }
            string numero = "";
            string reponse = "";

            do
            {
                Console.WriteLine("Avez-Vous un compte ? (oui/non)");
                reponse = Console.ReadLine();

            } while (reponse != "oui" && reponse != "non");

            if (reponse == "non")
            {
                maConnexion.Close();

                numero = Inscription(maConnexion, cdr);
            }
            else if (reponse == "oui")
            {
                maConnexion.Close();

                numero = Connexion(maConnexion, cdr);
            }


            maConnexion.Close();
            return numero;
        }
        /// <summary>
        /// genere aléatoirement un numero client ou cdr 
        /// </summary>
        /// <returns></numéro client>
        static string generateID()
        {
            long i = 1;

            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }

            string number = String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);

            return number;
        }
        /// <summary>
        /// Inscription si l'individu n'est pas dans la BDD
        /// </summary>
        /// <param name="maConnexion"></param>
        /// <param name="cdr"></booléen CdR>
        /// <returns></le numéro du membre>
        static string Inscription(MySqlConnection maConnexion, bool cdr)
        {
            maConnexion.Open();
            Console.Clear();
            Console.WriteLine("-----------------------------------------Espace Création de compte------------------------------------------\n");
            Console.WriteLine("Adresse : ");
            string adresse = Console.ReadLine();
            Console.WriteLine("Nom : ");
            string nom = Console.ReadLine();
            Console.WriteLine("email : ");
            string mail = Console.ReadLine();
            string num = generateID();
            maConnexion.Close();
            if (cdr == false)
            {
                Ajout_Client(maConnexion, nom, adresse, num, mail);
            }
            else
            {
                Ajout_CdR(maConnexion, num, nom, adresse, mail, 0);
            }
            maConnexion.Close();
            Console.Clear();
            return num;
        }
        /// <summary>
        /// methode permettant de retrouver le client dans la BDD
        /// </summary>
        /// <param name="maConnexion"></param>
        /// <param name="Cdr"></booléens du CdR>
        /// <returns></numéro du membre>
        static string Connexion(MySqlConnection maConnexion, bool Cdr)
        {
            maConnexion.Open();
            Console.Clear();
            Console.WriteLine("-----------------------------------------Espace Connexion------------------------------------------\n");
            string num = "";
            int verif = 0;
            do
            {


                Console.Write("Veuillez entrer votre adresse email : ");
                string mail = Console.ReadLine();
                Console.Write("\nVeuillez entrer votre numéro client : ");

                num = Console.ReadLine();
                string requete = "";
                if (Cdr == false)
                {

                    requete = "SELECT EXISTS (SELECT Num_Client,email FROM Client WHERE Num_Client='" + num + "' AND email='" + mail + "');";
                }
                else
                {
                    requete = "SELECT EXISTS (SELECT Num_Cdr ,email_CdR FROM CréateurDeRecettes WHERE Num_Cdr='" + num + "' AND email_CdR='" + mail + "');";
                }

                MySqlCommand command1 = maConnexion.CreateCommand();
                command1.CommandText = requete;
                MySqlDataReader reader = command1.ExecuteReader();

                while (reader.Read()) // parcours ligne par ligne
                {
                    verif = reader.GetInt32(0);
                    if (verif == 1)
                    {
                        Console.Clear();
                        Console.WriteLine("\n****** Access Granted ******\n");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("\n****** Identifiant ou email incorrect(s) ******\n");

                    }
                }
                reader.Close();

            } while (verif != 1);



            maConnexion.Close();


            return num;

        }
        /// <summary>
        /// methode pour passer une commande
        /// </summary>
        /// <param name="maConnexion"></param>
        /// <param name="cdr"></booléens cdr>
        static void Commander(MySqlConnection maConnexion, bool cdr)
        {

            string numero = Identification(maConnexion, cdr);
            maConnexion.Open();

            Console.WriteLine("Au menu aujourd'hui : \n");
            string requete = "SELECT * FROM Recette ;";

            MySqlCommand command1 = maConnexion.CreateCommand();
            command1.CommandText = requete;
            MySqlDataReader reader = command1.ExecuteReader();
            string desc = "";
            string Nom = "";
            string type = "";
            float prix = 0;
            while (reader.Read())// parcours ligne par ligne
            {
                desc = reader.GetString(0); //récupération de la 1ère colonne
                Nom = reader.GetString(1); //récupération de la 2eme colonne
                type = reader.GetString(2); //récupération de la 3eme colonne
                prix = reader.GetInt32(3); //récupération de la 4eme colonne


                Console.WriteLine(desc + " : Nom:" + Nom + " , type:" + type + " , prix:" + prix);



            }
            reader.Close();
            bool choix = false;
            List<string> liste = new List<string>();

            string requete2 = "SELECT Nom_Recette,prix FROM Recette;";

            MySqlCommand command2 = maConnexion.CreateCommand();
            command2.CommandText = requete2;
            MySqlDataReader reader2 = command2.ExecuteReader();
            string nom2 = "";
            int price = 0;
            List<string> nomRecette = new List<string>();
            List<int> LesPrix = new List<int>();
            while (reader2.Read())
            {
                nom2 = reader2.GetString(0);
                price = reader2.GetInt32(1);
                nomRecette.Add(nom2);
                LesPrix.Add(price);
            }
            reader2.Close();
            price = 0;
            bool bonus = false;



            do
            {
                int n = 0;
                Console.Write("\nSelectionnez le nom de la recette désirée :");

                string recette = Console.ReadLine();
                if (nomRecette.Contains(recette))   //delegate pour checke la presence de la liste
                {
                    for (int i = 0; i < nomRecette.Count(); i++)
                    {
                        if (recette == nomRecette[i])
                        {
                            Console.Write("Selectionnez une quantité : ");
                            n = Convert.ToInt32(Console.ReadLine());        //compteur de recette
                            int nbis = n;
                            maConnexion.Close();
                            maConnexion.Open();

                            // Acréditation des points Cook au CdR par commande de sa recette
                            string sql = "update  CréateurDeRecettes,Recette set points_Cook = @points_Cook where Nom_Recette = '" + recette + "' and ID_CdR= Num_CdR ;";
                            MySqlCommand command5 = maConnexion.CreateCommand();

                            MySqlParameter points_Cook = new MySqlParameter("@points_Cook", MySqlDbType.Int32);
                            command5.CommandText = sql;

                            string requetepoint = "SELECT points_Cook FROM CréateurDeRecettes JOIN Recette on Num_CdR=ID_CdR WHERE Nom_Recette = '" + recette + "';";
                            MySqlCommand commandPoint = maConnexion.CreateCommand();
                            commandPoint.CommandText = requetepoint;
                            MySqlDataReader readerpoint = commandPoint.ExecuteReader();
                            int pts = 0;

                            while (readerpoint.Read())
                            {
                                pts = readerpoint.GetInt32(0);
                            }
                            readerpoint.Close();
                            pts += n * 1;   //rémuneration de 1 cook pour chaque commande de sa recette
                            points_Cook.Value = pts;

                            command5.Parameters.Add(points_Cook);

                            command5.ExecuteNonQuery();


                            requete2 = "SELECT prix FROM Recette WHERE Nom_Recette = " + recette + ";";
                            command2.CommandText = requete2;
                            price += LesPrix[i] * n;
                            if (n > 10 && n <= 50)
                            {
                                price += 2;
                            }
                            else if (n > 50)
                            {
                                bonus = true;

                                price += 5;
                            }
                            while (n != 0)
                            {
                                liste.Add(recette);
                                n--;
                                Historique_Commande(maConnexion, DateTime.Now, numero, recette);
                                MaJ_Stock(maConnexion, recette); //Mise à jour des stocks
                                maConnexion.Close();
                            }

                        }
                    }
                }
                else
                {
                    Console.WriteLine("Desolé, cette recette n'est pas valide :(");
                }




                Console.WriteLine("Souhaitez vous sélectionner une autre recette ? (true/false)");
                choix = Convert.ToBoolean(Console.ReadLine());      //repondre par true/false
            } while (choix != false);

            Console.Clear();

            Console.WriteLine("Voici le récapitulatif de votre commande pour un total de : " + price);
            foreach (string el in liste)
            {
                Console.WriteLine(el);
            }
            Console.WriteLine("Souhaitez-vous valider votre commande ? (oui/non)");
            string rep = Console.ReadLine();
            if (rep == "oui")
            {
                Console.Clear();
                if (Paiement(maConnexion, numero, price, bonus))
                {
                    Console.WriteLine("Paiement effectué avec succès. Merci de votre commande ! ");
                }
                else
                {
                    Console.WriteLine("Paiement refusé. Solde insuffisant. ");
                }
            }



            maConnexion.Close();

        }


        /// <summary>
        /// Actualisation des valeurs de stock des produits composant une recette
        /// </summary>
        /// <param name="maConnexion"></param>
        /// <param name="nom_r"></nom de la recette>
        static void MaJ_Stock(MySqlConnection maConnexion, string nom_r)
        {
            maConnexion.Open();
            string requete = "SELECT stock_actuel, unité from Produit " +
                "where categorie = '" + nom_r + "';";

            MySqlCommand command1 = maConnexion.CreateCommand();
            command1.CommandText = requete;

            MySqlDataReader reader2 = command1.ExecuteReader();
            List<int> stock = new List<int>();
            List<int> qté = new List<int>();
      
            while (reader2.Read())
            {
                stock.Add(reader2.GetInt32(0));
                qté.Add(reader2.GetInt32(1));
            }
            for (int j = 0; j < stock.Count; j++)
            {
                stock[j] -= qté[j];
            }
            reader2.Close();

            string req = "SELECT Nom_Prod FROM Produit where categorie = '" + nom_r + "';";
            MySqlCommand command2 = maConnexion.CreateCommand();
            command2.CommandText = req;

            MySqlDataReader reader = command2.ExecuteReader();
            List<string> name = new List<string>();
            while (reader.Read())
            {
                name.Add(reader.GetString(0));
            }


            reader.Close();
            MySqlCommand command4 = maConnexion.CreateCommand();
            command4.CommandText = requete;

            int n = 1;
            
            for (int i = 0; i < stock.Count; i++)
            {
                string sql = "update  produit set stock_actuel = @stock_actuel where categorie = '" + nom_r + "' and Nom_Prod= '" + name[i] + "' ;";
                MySqlCommand command5 = maConnexion.CreateCommand();


                MySqlParameter stock_actuel = new MySqlParameter("@stock_actuel", MySqlDbType.Int32);
                command5.CommandText = sql;
                stock_actuel.Value = stock[i];

                command5.Parameters.Add(stock_actuel);

                command5.ExecuteNonQuery();

                n++;
            }


            maConnexion.Close();


        }
        /// <summary>
        /// methode simulant le paiement
        /// </summary>
        /// <param name="maConnexion"></param>
        /// <param name="num"></numero du membre>
        /// <param name="facturation"></montant de la facture>
        /// <param name="verifBonus"></bonus pour le cdr dont la recette est commandée > 50>
        /// <returns></true si le solde est suffisant alors le paiement peur s'effectuer>
        public static bool Paiement(MySqlConnection maConnexion, string num, int facturation, bool verifBonus)
        {
            try
            {
                maConnexion.Open();
            }
            catch (Exception e) { }
            string requete = "SELECT points_Cook FROM CréateurDeRecettes WHERE Num_CdR=" + num + ";";
            MySqlCommand command1 = maConnexion.CreateCommand();
            command1.CommandText = requete;
            MySqlDataReader reader = command1.ExecuteReader();
            int pts = 0;
            int pointBonus = 0;
            string[] valueString = new string[reader.FieldCount];

            if (verifBonus)
            {
                pointBonus += 4;
            }


            while (reader.Read())
            {

                pts = reader.GetInt32(0);
            }
            if (pts >= facturation)
            {
                int solde = pts - facturation + pointBonus;

                string sql = "Update CréateurDeRecettes set points_Cook=@points_Cook WHERE Num_CdR=" + num;

                MySqlCommand command4 = maConnexion.CreateCommand();
                command4.CommandText = sql;
                MySqlParameter points_Cook = new MySqlParameter("@points_Cook", MySqlDbType.Int32);
                points_Cook.Value = solde;
                command4.Parameters.Add(points_Cook);

                reader.Close();
                int numrowsUpdated = command4.ExecuteNonQuery();
                maConnexion.Close();

                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// Affiche le createur de recette de la semaine
        /// </summary>
        /// <param name="maConnexion"></param>
        static void Classement_Hebdomadaire(MySqlConnection maConnexion)
        {
            try
            { maConnexion.Open(); }
            catch(Exception e) { }
            string requete = "SELECT  Nom_CdR,count(*)  FROM créateurderecettes JOIN Recette ON ID_CdR=Num_CdR JOIN historique_client ON Nom_Recette = Recette_Commandée " +
                "WHERE WEEK(Date_Commande)  IN(SELECT WEEK(now())week) " +
                " GROUP BY Nom_Cdr ORDER BY count(*) DESC LIMIT 1; ";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("Voici le créateur de recette de la semaine : " + reader.GetValue(0) + " avec un total de " + reader.GetValue(1) + " commandes ! ");
            }
            reader.Close();

            maConnexion.Close();
        }
        /// <summary>
        /// Affiche le top 5 des recettes ayant été commandée
        /// </summary>
        /// <param name="maConnexion"></param>
        static void Top5_Recette(MySqlConnection maConnexion)
        {
            maConnexion.Open();
            string requete = "SELECT Recette_Commandée,type ,Nom_CdR FROM historique_client JOIN Recette ON Recette_Commandée=Nom_Recette " +
                "JOIN CréateurDeRecettes ON ID_CdR = Num_CdR GROUP BY Recette_Commandée ORDER BY count(*) DESC LIMIT 5; ";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("Nom : " + reader.GetValue(0) + " / type : " + reader.GetValue(1) + " / créateur : " + reader.GetValue(2));
            }
            reader.Close();
            maConnexion.Close();
        }
        /// <summary>
        /// retrouve le cdr d'or et affiche ses 5 recettes les plus commandées
        /// </summary>
        /// <param name="maConnexion"></param>
        static void CdR_Or(MySqlConnection maConnexion)
        {
            maConnexion.Open();

            string requete = "SELECT Nom_CdR FROM créateurderecettes JOIN recette ON Num_CdR=ID_CdR JOIN historique_client ON Recette_Commandée = Nom_Recette " +
                "GROUP BY Nom_CdR ORDER BY count(*) DESC LIMIT 1; ";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            string nom = "";
            while (reader.Read())
            {
                nom = reader.GetString(0);
            }
            reader.Close();

            string requete2 = "SELECT Recette_Commandée FROM historique_client JOIN Recette ON Recette_Commandée=Nom_Recette " +
                "JOIN créateurderecettes ON ID_CdR = Num_CdR WHERE Nom_CdR = '" + nom + "' GROUP BY Recette_Commandée ORDER by count(*) DESC LIMIT 5; ";
            MySqlCommand command2 = maConnexion.CreateCommand();
            command2.CommandText = requete2;
            MySqlDataReader reader2 = command2.ExecuteReader();
            Console.WriteLine("Le CdR d'Or est " + nom + "\nVoici ses 5 recettes les plus commandées : ");
            while (reader2.Read())
            {
                Console.WriteLine(reader2.GetValue(0));
            }
            reader2.Close();

            maConnexion.Close();
        }
        /// <summary>
        /// methode qui efface un cdr de la base cdr mais il reste client 
        /// </summary>
        /// <param name="maConnexion"></param>
        /// <param name="num"></numero du membre à retirer>
        static void Supprimer_CdR(MySqlConnection maConnexion, string num)
        {
            maConnexion.Open();

            string requete = "DELETE FROM créateurderecettes WHERE Num_CdR='" + num + "'; ";
            MySqlCommand command1 = maConnexion.CreateCommand();
            command1.CommandText = requete;
            command1.ExecuteNonQuery();
            Console.WriteLine("Suppression effectuée");

            maConnexion.Close();
        }
        /// <summary>
        /// supprime la ou les recettes
        /// </summary>
        /// <param name="maConnexion"></param>
        /// <param name="num"></numero du cdr >
        /// <param name="suppression_totale"></false par defaut mais si true alors on supprime toutes les recettes du cdr>
        static void Supprimer_Recette(MySqlConnection maConnexion, string num, bool suppression_totale = false)
        {

            maConnexion.Open();

            string r = "SELECT Nom_Recette FROM Recette WHERE ID_CdR='" + num + "'; ";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = r;
            MySqlDataReader reader = command.ExecuteReader();


            string requete = "";
            if (suppression_totale == true)
            {
                requete = "DELETE FROM Recette WHERE ID_CdR='" + num + "';";
            }
            else
            {
                Console.WriteLine("Voici la liste de vos recettes : ");
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetValue(0));
                }
                Console.Write("Quelle recette voulez-vous supprimer ? : ");
                string nom = Console.ReadLine();
                requete = "DELETE FROM Recette WHERE Nom_Recette='" + nom + "';";
            }
            reader.Close();
            MySqlCommand command1 = maConnexion.CreateCommand();
            command1.CommandText = requete;
            command1.ExecuteNonQuery();
            Console.WriteLine("Suppression effectuée");

            maConnexion.Close();
        }
        /// <summary>
        /// Divise par 2 les quantités de produits qui n'ont pas été utilisé depuis plus de 30 jours
        /// </summary>
        /// <param name="maConnexion"></param>
        static void Peremption(MySqlConnection maConnexion)
        {
            maConnexion.Open();
            string requete = "update produit set stock_actuel=stock_actuel/2,stock_min=stock_min/2,stock_max=stock_max/2 " +
                "where categorie in (select Recette_Commandée from historique_client " +
                "where Recette_Commandée not in ( select Recette_Commandée from historique_client " +
                "where dayofyear(Date_Commande) <= dayofyear(now()) and dayofyear(Date_Commande) >= (dayofyear(now()) - 30))); ";

            MySqlCommand command5 = maConnexion.CreateCommand();


            MySqlParameter stock_actuel = new MySqlParameter("@stock_actuel", MySqlDbType.Int32);
            command5.CommandText = requete;
            stock_actuel.Value = stock_actuel;

            command5.Parameters.Add(stock_actuel);

            command5.ExecuteNonQuery();
        }
        /// <summary>
        /// fonction qui ecrit l'inventaire des commandes de réapprovisionnement dans le document XML
        /// </summary>
        /// <param name="maConnexion"></param>
        static void Commande_Fournisseur(MySqlConnection maConnexion)
        {
            maConnexion.Open();
            XmlDocument docXml = new XmlDocument();
            XmlElement racine = docXml.CreateElement("liste_des_commandes");
            docXml.AppendChild(racine);

            XmlDeclaration xmldecl = docXml.CreateXmlDeclaration("1.0", "UTF-8", "no");
            docXml.InsertBefore(xmldecl, racine);
            string req_prod = "SELECT Nom_Fournisseur,Num_Fournisseur,Nom_Prod,stock_actuel,stock_max" +
                " FROM produit, Fournisseur WHERE Num_Fournisseur = reference AND stock_actuel<stock_min AND reference IN (SELECT reference" +
                " FROM produit WHERE stock_actuel < stock_min); ";
            MySqlCommand command_prod = maConnexion.CreateCommand();
            command_prod.CommandText = req_prod;
            MySqlDataReader reader_prod = command_prod.ExecuteReader();
            string nom_f = "";
            string refe= "";
            string nom_p = "";
            int stocka = 0;
            int stockmx = 0;
           
            while (reader_prod.Read())
            {
                nom_f = reader_prod.GetString(0);
                refe = reader_prod.GetString(1);
                nom_p = reader_prod.GetString(2);
                stocka = reader_prod.GetInt32(3);
                stockmx = reader_prod.GetInt32(4);

                XmlElement Fournisseur = docXml.CreateElement(nom_f);
                racine.AppendChild(Fournisseur);

                XmlAttribute Balise_Num= docXml.CreateAttribute("Num_Tel");
                Balise_Num.InnerText = refe;
                Fournisseur.SetAttributeNode(Balise_Num);

                XmlElement Balise_Produit = docXml.CreateElement("Nom_Produit");
                Balise_Produit.InnerText =nom_p;
                Fournisseur.AppendChild(Balise_Produit);
               

                XmlElement Balise_Qté = docXml.CreateElement("Quantité_à_commander");
                int stock = stockmx - stocka;
                Balise_Qté.InnerText = stock.ToString();
                Balise_Produit.AppendChild(Balise_Qté);

                
            }

            reader_prod.Close();
            


            docXml.Save("liste_commandes.xml");
            maConnexion.Close();
        }
    }
}
