using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Keepr.Models;

namespace Keepr.Repositories
{
  public class VaultsRepository
  {
    private readonly IDbConnection _db;

    public VaultsRepository(IDbConnection db)
    {
      _db = db;
    }

    internal List<Vault> GetAllProfileVaults(string id)
    {
      string sql = @"SELECT v.*, a.* FROM vaults v JOIN accounts a ON v.creatorId = a.id WHERE v.creatorId = @id ORDER BY v.id desc;";
      List<Vault> vaults = _db.Query<Vault, Profile, Vault>(sql, (vault, profile) => {
        vault.Creator = profile;
        return vault;}, new {id}).ToList();
      return vaults;
    }

    internal Vault GetVaultById(int id)
    {
      // this V seems pointless, only the vault creator can access the vault, so why would they want to see creator info?
      string sql = @"SELECT 
        v.*,
        a.* 
        FROM vaults v 
        JOIN accounts a ON v.creatorId = a.id
        WHERE v.id = @id;";
      Vault vault = _db.Query<Vault, Profile, Vault>(sql, (vault, profile) => {
          vault.Creator = profile;
          return vault;
        }, new {id}).FirstOrDefault();
      return vault;
    }

    internal Vault CreateVault(Vault vaultData)
    {
      string sql = @"INSERT INTO vaults
        (creatorId, name, description, img, isPrivate)
        VALUES
        (@creatorId, @name, @description, @img, @isPrivate);
        SELECT LAST_INSERT_ID();";
        int id = _db.ExecuteScalar<int>(sql, vaultData);
        vaultData.Id = id;
        return vaultData;
    }

    internal Vault EditVault(Vault edit)
    {
      string sql = @"
      UPDATE vaults SET
      name = @name,
      description = @description,
      img = @img,
      isPrivate = @isPrivate
      WHERE id = @id;";
      _db.Execute(sql, edit);
      return edit;
    }

    internal void DeleteVault(int id)
    {
      string sql = "DELETE FROM vaults WHERE id = @id;";
      _db.Execute(sql, new {id});
    }
  }
}