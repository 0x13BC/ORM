using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace com.castsoftware.tools
{
    /// <summary>
    /// La classe ne contient que des m�thodes statiques.
    /// </summary>
    internal static partial class DPersistanceExemple
    {
        //#region M�thodes
        ///// <summary>
        ///// Charge les informations d'un {0}
        ///// </summary>
        ///// <param name="p_identifiant">
        ///// JPF Il doit y avoir autant de param�tres que d'�l�ments dans la PK.
        ///// BLABLA
        ///// </param>
        ///// <returns>
        ///// Une instance de {0} r�cup�r� dans la base � partir de sa clef primaire.
        ///// </returns>
        //public static D{0} SCharge{0}(int p_{...})
        //{
        //    using (SqlConnection l_connection = TFournisseurConnexion.RetourneConnexion(TransactionScopeOption.Suppress))
        //    {
        //        D{0} l_resultat = new D{0}();
        //        try
        //        {
        //            // JPF On appelle la PS en s�lection sur PK.
        //            SqlCommand l_commande = new SqlCommand("VUSRDMS0_S_Utilisateur", l_connection);
        //            l_commande.CommandType = CommandType.StoredProcedure;
        //            // JPF Remplacer le nom du param�tre de la PS.
        //            // JPF Autant de ligne ci-dessous que de param�tre de la PS
        //            l_commande.Parameters.Add(new SqlParameter("@ID", p_{...}));
        //            SqlDataReader l_reader = l_commande.ExecuteReader();

        //            if (l_reader.Read())
        //            {
        //                // JPF Autant de lignes ci-dessous que d'attribut dans l'objet.
        //                // JPF Le GetXXX d�pend du type de la colonne r�cup�r�e.
        //                // JPF Attention l'ordre est important. Il doit concorder avec celui des
        //                // colonnes renvoy�es par la PS.
        //                l_resultat.Id = new int?(l_reader.GetInt32(0));
        //                // JPF Attention tester IsDBNull sur les colonnes nullisables.
        //                l_resultat.Login = l_reader.IsDBNull(3) ? null : l_reader.GetString(1);
        //                l_resultat.Nom = l_reader.GetString(2);
        //                l_resultat.Prenom = l_reader.GetString(3);
        //                l_resultat.DateDebutValidite = l_reader.GetDateTime(4);
        //                l_resultat.DateFinValidite = l_reader.GetDateTime(5);
        //            }

        //        }
        //        catch (SqlException e) { throw new .TExceptionTechnique(e); }
        //        return l_resultat;
        //    }
        //}
        ///// <summary>
        ///// Ins�re un {0} dans la base de donn�es
        ///// </summary>
        ///// <param name="p_utilisateur">
        ///// Le {0} � ins�rer.
        ///// </param>
        //public static void SInsere{0}(ref D{0} p_{...})
        //{
        //    using (SqlConnection l_connection = TFournisseurConnexion.RetourneConnexion(TransactionScopeOption.Suppress))
        //    {
        //        try
        //        {
        //            // JPF On appelle la PS en insertion. ATTENTION � changer le nom de la PS
        //            SqlCommand l_commande = new SqlCommand("VUSRDMS0_I_{0}", l_connection);
        //            l_commande.CommandType = CommandType.StoredProcedure;
        //            // JPF voir remarques plus haut pour le SELECT
        //            l_commande.Parameters.Add(new SqlParameter("@LOGN", p_utilisateur.Login));
        //            l_commande.Parameters.Add(new SqlParameter("@NOM", p_utilisateur.Nom));
        //            l_commande.Parameters.Add(new SqlParameter("@PNOM", p_utilisateur.Prenom));
        //            l_commande.Parameters.Add(new SqlParameter("@VALI_DD", p_utilisateur.DateDebutValidite));
        //            l_commande.Parameters.Add(new SqlParameter("@VALI_DF", p_utilisateur.DateFinValidite));

        //            // JPF D�finition du param�tre en sortie de la PS
        //            SqlParameter l_sqlparam = new SqlParameter("@@identity", SqlDbType.Int, 4);
        //            l_sqlparam.Direction = ParameterDirection.ReturnValue;
        //            l_commande.Parameters.Add(l_sqlparam);

        //            // JPF Attention on n'ex�cute pas un reader.
        //            l_commande.ExecuteNonQuery();

        //            // JPF R�cup�ration de l'identifiant de l'objet cr�� en sortie de la PS
        //            p_{0}.Id = new int?(Convert.ToInt32(l_commande.Parameters["@@identity"].Value));

        //        }
        //        catch (SqlException e) { throw new TExceptionTechnique(e); }
        //    }
        //}

        ///// <summary>
        ///// Modifie un {0} dans la base de donn�es
        ///// </summary>
        ///// <param name="p_utilisateur">
        ///// Le {0} � ins�rer.
        ///// </param>
        //public static void SModifie{0}(D{0} p_{...})
        //{
        //    using (SqlConnection l_connection = TFournisseurConnexion.RetourneConnexion(TransactionScopeOption.Suppress))
        //    {
        //        try
        //        {
        //            // JPF On appelle la PS en update. ATTENTION � changer le nom de la PS
        //            SqlCommand l_commande = new SqlCommand("VUSRDMS0_U_Utilisateur", l_connection);
        //            l_commande.CommandType = CommandType.StoredProcedure;
        //            // JPF voir remarques plus haut pour le SELECT
        //            l_commande.Parameters.Add(new SqlParameter("@ID", p_utilisateur.Id));
        //            l_commande.Parameters.Add(new SqlParameter("@LOGN", p_utilisateur.Login));
        //            l_commande.Parameters.Add(new SqlParameter("@NOM", p_utilisateur.Nom));
        //            l_commande.Parameters.Add(new SqlParameter("@PNOM", p_utilisateur.Prenom));
        //            l_commande.Parameters.Add(new SqlParameter("@VALI_DD", p_utilisateur.DateDebutValidite));
        //            l_commande.Parameters.Add(new SqlParameter("@VALI_DF", p_utilisateur.DateFinValidite));
        //            // JPF Attention on n'ex�cute pas un reader.
        //            l_commande.ExecuteNonQuery();
        //        }
        //        catch (SqlException e) { throw new TExceptionTechnique(e); }
        //    }
        //}

        ///// <summary>
        ///// Supprime un {0} dans la base de donn�es
        ///// </summary>
        ///// <param name="p_utilisateur">
        ///// Le {0} � supprimer.
        ///// </param>
        //public static void SSupprime{0}(int p_identifiant)
        //{
        //    using (SqlConnection l_connection = TFournisseurConnexion.RetourneConnexion(TransactionScopeOption.Suppress))
        //    {
        //        try
        //        {
        //            // JPF On appelle la PS en update. ATTENTION � changer le nom de la PS
        //            SqlCommand l_commande = new SqlCommand("VUSRDMS0_D_Utilisateur", l_connection);
        //            l_commande.CommandType = CommandType.StoredProcedure;
        //            // JPF seuls sont pass�s les param�tres identifiants de l'objet.
        //            l_commande.Parameters.Add(new SqlParameter("@ID", p_identifiant));
        //            // JPF Attention on n'ex�cute pas un reader.
        //            l_commande.ExecuteNonQuery();
        //        }
        //        catch (SqlException e) { throw new TExceptionTechnique(e); }
        //    }
        //}
        //#endregion
    }
}
