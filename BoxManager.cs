/*
TShock, a server mod for Terraria
Copyright (C) 2011-2012 The TShock Team

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml;
using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;
using Terraria;

namespace Boxes
{
	public class BoxManager
	{
		public List<Box> AllBoxes = new List<Box>();
		public static string TableName = "Boxes";

		private IDbConnection database;

		public BoxManager(IDbConnection db)
		{
			database = db;
			var table = new SqlTable(TableName,
			                         new SqlColumn("X1", MySqlDbType.Int32),
			                         new SqlColumn("Y1", MySqlDbType.Int32),
			                         new SqlColumn("width", MySqlDbType.Int32),
			                         new SqlColumn("height", MySqlDbType.Int32),
			                         new SqlColumn("BoxName", MySqlDbType.VarChar, 50) {Primary = true},
			                         new SqlColumn("WorldID", MySqlDbType.Text),
			                         new SqlColumn("UserIds", MySqlDbType.Text),
			                         new SqlColumn("Protected", MySqlDbType.Int32),
			                         new SqlColumn("Groups", MySqlDbType.Text),
			                         new SqlColumn("Owner", MySqlDbType.VarChar, 50),
                                     new SqlColumn("Z", MySqlDbType.Int32){ DefaultValue = "0" }
				);
			var creator = new SqlTableCreator(db,
			                                  db.GetSqlType() == SqlType.Sqlite
			                                  	? (IQueryBuilder) new SqliteQueryCreator()
			                                  	: new MysqlQueryCreator());
			creator.EnsureExists(table);

			ReloadAllBoxes();
		}

		public void ReloadAllBoxes()
		{
			try
			{
				ReloadForMap(Main.worldID.ToString());
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
		}

		public void ReloadForMap(String map)
		{
			using (var reader = database.QueryReader("SELECT * FROM Boxes WHERE WorldID=@0", map))
			{
				AllBoxes.Clear();
				while (reader.Read())
				{
					int X1 = reader.Get<int>("X1");
					int Y1 = reader.Get<int>("Y1");
					int height = reader.Get<int>("height");
					int width = reader.Get<int>("width");
					int Protected = reader.Get<int>("Protected");
					string mergedids = reader.Get<string>("UserIds");
					string name = reader.Get<string>("BoxName");
					string owner = reader.Get<string>("Owner");
					string groups = reader.Get<string>("Groups");
					int z = reader.Get<int>("Z");
					
					string[] splitids = mergedids.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
					
					Box r = new Box(new Rectangle(X1, Y1, width, height), name, owner, Protected != 0, Main.worldID.ToString(), z);
					r.SetAllowedGroups(groups);
					try
					{
						for (int i = 0; i < splitids.Length; i++)
						{
							int id;
							
							if (Int32.TryParse(splitids[i], out id)) // if unparsable, it's not an int, so silently skip
								r.AllowedIDs.Add(id);
							else
								Log.Warn("One of your UserIDs is not a usable integer: " + splitids[i]);
						}
					}
					catch (Exception e)
					{
						Log.Error("Your database contains invalid UserIDs (they should be ints).");
						Log.Error("A lot of things will fail because of this. You must manually delete and re-create the allowed field.");
						Log.Error(e.ToString());
						Log.Error(e.StackTrace);
					}
					
					AllBoxes.Add(r);
				}
			}
		}

		public bool AddBox(int tx, int ty, int width, int height, string boxname, string owner, string worldid, int z = 0)
		{
			if (GetBoxByName(boxname) != null)
			{
				return false;
			}
			try
			{
				database.Query(
					"INSERT INTO Boxes (X1, Y1, width, height, BoxName, WorldID, UserIds, Protected, Groups, Owner, Z) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10);",
					tx, ty, width, height, boxname, worldid, "", 1, "", owner, z);
				AllBoxes.Add(new Box(new Rectangle(tx, ty, width, height), boxname, owner, true, worldid, z));
				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return false;
		}

		public bool DeleteBox(string name)
		{
			try
			{
				database.Query("DELETE FROM Boxes WHERE BoxName=@0 AND WorldID=@1", name, Main.worldID.ToString());
				var worldid = Main.worldID.ToString();
				AllBoxes.RemoveAll(r => r.Name == name && r.WorldID == worldid);
				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return false;
		}

		public bool SetBoxProtected(string name, bool state)
		{
			try
			{
				return SetBoxProtectedForWorld(name, state, Main.worldID.ToString());
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return false;
			}
		}

		public bool SetBoxProtectedForWorld(string name, bool state, string world)
		{
			database.Query("UPDATE Boxes SET Protected=@0 WHERE BoxName=@1 AND WorldID=@2", state ? 1 : 0, name, world);
			var box = GetBoxByName(name);
			if (box != null)
				box.DisableBuild = state;
			return true;

		}

		public bool CanBuild(int x, int y, TSPlayer ply)
		{
			if (!ply.Group.HasPermission(Permissions.canbuild))
			{
				return false;
			}
		    Box top = null;
			for (int i = 0; i < AllBoxes.Count; i++)
			{
				if (AllBoxes[i].InArea(x,y) )
				{
                    if (top == null)
                        top = AllBoxes[i];
                    else
                    {
                        if (AllBoxes[i].Z > top.Z)
                            top = AllBoxes[i];
                    }
				}
			}
            return top == null || top.HasPermissionToBuildInBox(ply);
		}

		public bool InArea(int x, int y)
		{
			foreach (Box region in AllBoxes)
			{
				if (x >= region.Area.Left && x <= region.Area.Right &&
				    y >= region.Area.Top && y <= region.Area.Bottom &&
				    region.DisableBuild)
				{
					return true;
				}
			}
			return false;
		}

        public List<string> InAreaBoxName(int x, int y)
        {
            List<string> boxes = new List<string>() { };
            foreach (Box box in AllBoxes)
            {
                if (x >= box.Area.Left && x <= box.Area.Right &&
                    y >= box.Area.Top && y <= box.Area.Bottom &&
                    box.DisableBuild)
                {
                    boxes.Add(box.Name);
                }
            }
            return boxes;
        }

        public List<Box> InAreaBox(int x, int y)
        {
            List<Box> regions = new List<Box>() { };
            foreach (Box region in AllBoxes)
            {
                if (x >= region.Area.Left && x <= region.Area.Right &&
                    y >= region.Area.Top && y <= region.Area.Bottom &&
                    region.DisableBuild)
                {
                    regions.Add(region);
                }
            }
            return regions;
        }

		public static List<string> ListIDs(string MergedIDs)
		{
			return MergedIDs.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();
		}

		public bool resizeBox(string boxName, int addAmount, int direction)
		{
			//0 = up
			//1 = right
			//2 = down
			//3 = left
			int X = 0;
			int Y = 0;
			int height = 0;
			int width = 0;
			try
			{
				using (
					var reader = database.QueryReader("SELECT X1, Y1, height, width FROM Boxes WHERE BoxName=@0 AND WorldID=@1",
					                                  boxName, Main.worldID.ToString()))
				{
					if (reader.Read())
						X = reader.Get<int>("X1");
					width = reader.Get<int>("width");
					Y = reader.Get<int>("Y1");
					height = reader.Get<int>("height");
				}
				if (!(direction == 0))
				{
					if (!(direction == 1))
					{
						if (!(direction == 2))
						{
							if (!(direction == 3))
							{
								return false;
							}
							else
							{
								X -= addAmount;
								width += addAmount;
							}
						}
						else
						{
							height += addAmount;
						}
					}
					else
					{
						width += addAmount;
					}
				}
				else
				{
					Y -= addAmount;
					height += addAmount;
				}
				int q =
					database.Query(
						"UPDATE Boxes SET X1 = @0, Y1 = @1, width = @2, height = @3 WHERE BoxName = @4 AND WorldID=@5", X, Y, width,
						height, boxName, Main.worldID.ToString());
				if (q > 0)
					return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return false;
		}

		public bool RemoveUser(string regionName, string userName)
		{
			Box r = GetBoxByName(regionName);
			if (r != null)
			{
				r.RemoveID(TShock.Users.GetUserID(userName));
				string ids = string.Join(",", r.AllowedIDs);
				int q = database.Query("UPDATE Boxes SET UserIds=@0 WHERE BoxName=@1 AND WorldID=@2", ids,
				                       regionName, Main.worldID.ToString());
				if (q > 0)
					return true;
			}
			return false;
		}

		public bool AddNewUser(string regionName, String userName)
		{
			try
			{
				string MergedIDs = string.Empty;
				using (
					var reader = database.QueryReader("SELECT * FROM Boxes WHERE BoxName=@0 AND WorldID=@1", regionName,
					                                  Main.worldID.ToString()))
				{
					if (reader.Read())
						MergedIDs = reader.Get<string>("UserIds");
				}

				if (string.IsNullOrEmpty(MergedIDs))
					MergedIDs = Convert.ToString(TShock.Users.GetUserID(userName));
				else
					MergedIDs = MergedIDs + "," + Convert.ToString(TShock.Users.GetUserID(userName));

				int q = database.Query("UPDATE Boxes SET UserIds=@0 WHERE BoxName=@1 AND WorldID=@2", MergedIDs,
				                       regionName, Main.worldID.ToString());
				foreach (var r in AllBoxes)
				{
					if (r.Name == regionName && r.WorldID == Main.worldID.ToString())
						r.setAllowedIDs(MergedIDs);
				}
				return q != 0;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Gets all the regions names from world
		/// </summary>
		/// <param name="worldid">World name to get regions from</param>
		/// <returns>List of regions with only their names</returns>
		public List<Box> ListAllBoxes(string worldid)
		{
			var regions = new List<Box>();
			try
			{
				using (var reader = database.QueryReader("SELECT BoxName FROM Boxes WHERE WorldID=@0", worldid))
				{
					while (reader.Read())
						regions.Add(new Box {Name = reader.Get<string>("BoxName")});
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return regions;
		}

		public Box GetBoxByName(String name)
		{
			return AllBoxes.FirstOrDefault(r => r.Name.Equals(name) && r.WorldID == Main.worldID.ToString());
		}

		public Box ZacksGetBoxByName(String name)
		{
			foreach (Box r in AllBoxes)
			{
				if (r.Name.Equals(name))
					return r;
			}
			return null;
		}

		public bool ChangeOwner(string boxName, string newOwner)
		{
			var region = GetBoxByName(boxName);
			if (region != null)
			{
				region.Owner = newOwner;
				int q = database.Query("UPDATE Boxes SET Owner=@0 WHERE BoxName=@1 AND WorldID=@2", newOwner,
				                       boxName, Main.worldID.ToString());
				if (q > 0)
					return true;
			}
			return false;
		}

		public bool AllowGroup(string regionName, string groups)
		{
			string groupsNew = "";
			using (
				var reader = database.QueryReader("SELECT * FROM Boxes WHERE BoxName=@0 AND WorldID=@1", regionName,
				                                  Main.worldID.ToString()))
			{
				if (reader.Read())
					groupsNew = reader.Get<string>("Groups");
			}
			if (groupsNew != "")
				groupsNew += ",";
			groupsNew += groups;

			int q = database.Query("UPDATE Boxes SET Groups=@0 WHERE RegionName=@1 AND WorldID=@2", groupsNew,
			                       regionName, Main.worldID.ToString());

			Box r = GetBoxByName(regionName);
			if (r != null)
			{
				r.SetAllowedGroups(groupsNew);
			}
			else
			{
				return false;
			}

			return q > 0;
		}

		public bool RemoveGroup(string boxName, string group)
		{
			Box r = GetBoxByName(boxName);
			if (r != null)
			{
				r.RemoveGroup(group);
				string groups = string.Join(",", r.AllowedGroups);
				int q = database.Query("UPDATE Boxes SET Groups=@0 WHERE RegionName=@1 AND WorldID=@2", groups,
				                       boxName, Main.worldID.ToString());
				if (q > 0)
					return true;
			}
			return false;
		}

        public Box GetTopBox( List<Box> boxes )
        {
            Box ret = null;
            foreach( Box r in boxes)
            {
                if (ret == null)
                    ret = r;
                else
                {
                    if (r.Z > ret.Z)
                        ret = r;
                }
            }
            return ret;
        }

        public bool SetZ( string name, int z )
        {
            try
            {
				database.Query("UPDATE Boxes SET Z=@0 WHERE RegionName=@1 AND WorldID=@2", z, name,
                               Main.worldID.ToString());

                var region = GetBoxByName(name);
                if (region != null)
                    region.Z = z;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        }
		
		public int GetUsersBoxedTiles (string playerName)
		{
			var reader = database.QueryReader ("SELECT * FROM Boxes WHERE Owner=@0 AND WorldID=@1", playerName,
			                                  Main.worldID.ToString ());
			var i = 0;
			while (reader.Read ()) {
				i += (reader.Get<int>("height")*reader.Get<int>("height"));
			};
			return i;
		}
	}

	public class Box
	{
		public Rectangle Area { get; set; }
		public string Name { get; set; }
		public string Owner { get; set; }
		public bool DisableBuild { get; set; }
		public string WorldID { get; set; }
		public List<int> AllowedIDs { get; set; }
		public List<string> AllowedGroups { get; set; }
        public int Z { get; set; }

		public Box(Rectangle box, string name, string owner, bool disablebuild, string RegionWorldIDz, int z)
			: this()
		{
			Area = box;
			Name = name;
			Owner = owner;
			DisableBuild = disablebuild;
			WorldID = RegionWorldIDz;
		    Z = z;
		}

		public Box()
		{
			Area = Rectangle.Empty;
			Name = string.Empty;
			DisableBuild = true;
			WorldID = string.Empty;
			AllowedIDs = new List<int>();
			AllowedGroups = new List<string>();
		    Z = 0;
		}

		public bool InArea(Rectangle point)
		{
			if (Area.Contains(point.X, point.Y))
			{
				return true;
			}
			return false;
		}
		
		public bool InArea(int x, int y) //overloaded with x,y
		{
			
				if (x >= Area.Left && x <= Area.Right && y >= Area.Top && y <= Area.Bottom)
				{
					return true;
				}
			
			return false;
		}


		public bool HasPermissionToBuildInBox(TSPlayer ply)
		{
			if (!ply.IsLoggedIn)
			{
				if (!ply.HasBeenNaggedAboutLoggingIn)
				{
					ply.SendMessage("You must be logged in to take advantage of protected regions.", Color.Red);
					ply.HasBeenNaggedAboutLoggingIn = true;
				}
				return false;
			}
			if (!DisableBuild)
			{
				return true;
			}

			return AllowedIDs.Contains(ply.UserID) || AllowedGroups.Contains(ply.Group.Name) || Owner == ply.UserAccountName ||
			       ply.Group.HasPermission("manageregion");
		}

		public void setAllowedIDs(String ids)
		{
			String[] id_arr = ids.Split(',');
			List<int> id_list = new List<int>();
			foreach (String id in id_arr)
			{
				int i = 0;
				int.TryParse(id, out i);
				if (i != 0)
					id_list.Add(i);
			}
			AllowedIDs = id_list;
		}

		public void SetAllowedGroups(String groups)
		{
			// prevent null pointer exceptions
			if (!string.IsNullOrEmpty(groups))
			{
				List<String> groupArr = groups.Split(',').ToList();

				for (int i = 0; i < groupArr.Count; i++)
					groupArr[i] = groupArr[i].Trim();

				AllowedGroups = groupArr;
			}
		}

		public void RemoveID(int id)
		{
			var index = -1;
			for (int i = 0; i < AllowedIDs.Count; i++)
			{
				if (AllowedIDs[i] == id)
				{
					index = i;
					break;
				}
			}
			AllowedIDs.RemoveAt(index);
		}

		public bool RemoveGroup(string groupName)
		{
			return AllowedGroups.Remove(groupName);
		}
	}
}
