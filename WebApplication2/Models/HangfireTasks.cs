using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace WebApplication2.Models
{
    public  class HangfireTasks
    {
	    public static async Task ParseAllAlbumsVkAsync()
	    {
		    Console.WriteLine("Start");
			var active_user = 1;
			var userList = new List<string>
			{
				"2329557afb5eaa5f8280b747b1ca43320eee63e7098c0ed8fcf802d94ea3692ca8bfc0416547cb7b7e15c",
				"74d89552338d10e3a6ddec113d6c5a481542afe13f176a5514303459ed9625ab47a4f68beff9499222b11",
				"bb15ee18ad62811a5dbe158b26f7dd7edf30fbf0c42d5d8ecd42c49778b94e3b2e49009f569f4cca1c1b0"
			};

			var api = new VkApi();
			try
			{
				await api.AuthorizeAsync(new ApiAuthParams()
				{
					AccessToken = userList[active_user]
				});
			}
			catch (Exception ex)
			{
				await api.LogOutAsync();
				api = new VkApi();

				active_user++;
				await api.AuthorizeAsync(new ApiAuthParams()
				{
					AccessToken = userList[active_user]
				});
			}
			Console.WriteLine("Auth");
			var groupList = new List<GroupAlbum>
				{
					new GroupAlbum() {GroupId = -76629546, AlbumId = 203426992},
					new GroupAlbum() {GroupId = -76629546, AlbumId = 203426935},
					new GroupAlbum() {GroupId = -76629546, AlbumId = 203426857},
					new GroupAlbum() {GroupId = -11571122, AlbumId = 229924509},
					new GroupAlbum() {GroupId = -11571122, AlbumId = 218215712},
					new GroupAlbum() {GroupId = -11571122, AlbumId = 229924703},
					new GroupAlbum() {GroupId = -42520747, AlbumId = 265095887},
					new GroupAlbum() {GroupId = -42520747, AlbumId = 265095549},
					new GroupAlbum() {GroupId = -42520747, AlbumId = 255052787},
					new GroupAlbum() {GroupId = -13212026, AlbumId = 270419956},
					new GroupAlbum() {GroupId = -13212026, AlbumId = 270419996},
					new GroupAlbum() {GroupId = -13212026, AlbumId = 270419973}
				};
			var addList = new List<WeaponList>();
			var removeList = new List<WeaponList>();
			var weaponListDb = new List<WeaponList>();

			using (var db = new DbNorthwind())
			{
				weaponListDb = await db.WeaponList.ToListAsync();
			}
			Console.WriteLine("GetWeapon");
			foreach (var group in groupList)
			{
				var photos = new List<Photo>();
				try
				{
					photos = api.Photo.Get(new PhotoGetParams()
					{
						AlbumId = PhotoAlbumType.Id(group.AlbumId),
						Reversed = true,
						Extended = true,
						Count = 1000,
						OwnerId = group.GroupId
					}).ToList();
				}
				
				catch (Exception ex2)
				{
					continue;
				}
				Console.WriteLine("GetWeapon");
				
					
					foreach (var photo in photos
						.Where(w => weaponListDb.FirstOrDefault(p => p.Src == w.Sizes.OrderByDescending(w => w.Height).First().Src.ToString()) == null))
					{
						addList.Add(new WeaponList()
						{
							Text = photo.Text,
							PhotoId = (long)photo.Id,
							AlbumId = (long)photo.AlbumId,
							GroupId = (long)photo.OwnerId,
							Src = photo.Sizes.OrderByDescending(w => w.Height).First().Src.ToString(),
							StartTime = Convert.ToDateTime(photo.CreateTime)
						});
					}
					
					
					var localRemoveList = weaponListDb.Where(w => w.GroupId == group.GroupId
					                                       && w.AlbumId == group.AlbumId
					                                       && photos.FirstOrDefault(f => f.Id == w.PhotoId) == null).ToList();
					
					removeList.AddRange(localRemoveList);
					Console.WriteLine("AddRemoveAndAdd");

					
					
			}

			using (var db = new DbNorthwind())
			{
				db.BulkCopy(addList);
				await db.DeleteAsync(removeList);
			}
			Console.WriteLine("Ok!");


		}
	    
        public static async Task Work()
	    {
		    
		    
		    
		    var active_user = 0;
		    var userList = new List<string>
		    {
			    "2329557afb5eaa5f8280b747b1ca43320eee63e7098c0ed8fcf802d94ea3692ca8bfc0416547cb7b7e15c",
			    "74d89552338d10e3a6ddec113d6c5a481542afe13f176a5514303459ed9625ab47a4f68beff9499222b11",
			    "bb15ee18ad62811a5dbe158b26f7dd7edf30fbf0c42d5d8ecd42c49778b94e3b2e49009f569f4cca1c1b0"
		    };

		    var api = new VkApi();
		    await api.AuthorizeAsync(new ApiAuthParams()
		    {
				AccessToken = userList[active_user]
		    });

		    var groupList = new List<GroupAlbum>
		    {
				new GroupAlbum() {GroupId = -76629546, AlbumId = 203426992},
				new GroupAlbum() {GroupId = -76629546, AlbumId = 203426935},
				new GroupAlbum() {GroupId = -76629546, AlbumId = 203426857},
				new GroupAlbum() {GroupId = -11571122, AlbumId = 229924509},
				new GroupAlbum() {GroupId = -11571122, AlbumId = 218215712},
				new GroupAlbum() {GroupId = -11571122, AlbumId = 229924703},
				new GroupAlbum() {GroupId = -42520747, AlbumId = 265095887},
				new GroupAlbum() {GroupId = -42520747, AlbumId = 265095549},
				new GroupAlbum() {GroupId = -42520747, AlbumId = 255052787},
				new GroupAlbum() {GroupId = -13212026, AlbumId = 270419956},
				new GroupAlbum() {GroupId = -13212026, AlbumId = 270419996},
			    new GroupAlbum() {GroupId = -13212026, AlbumId = 270419973}

			};

		    foreach (var group in groupList)
		    {
			    var photos = api.Photo.Get(new PhotoGetParams()
			    {
				    AlbumId = PhotoAlbumType.Id(group.AlbumId),
				    Reversed = true,
				    Extended = true,
				    Count = 1000,
				    OwnerId = group.GroupId
			    });

			    using (var db = new DbNorthwind())
			    {
				    var weaponList = await db.WeaponList.ToListAsync();
						var photosWithComments = photos
								.Where(w => w.Text == "" && w.Comments.Count != 0).ToList();

						photosWithComments = photosWithComments
							.Where(w => weaponList.FirstOrDefault(f =>
											f.GroupId == w.OwnerId && f.AlbumId == w.AlbumId && f.PhotoId == w.Id) == null).ToList();
						var addList = new List<WeaponList>();
						foreach (var photo in photosWithComments)
						{
							
							var weapon = new WeaponList();
							try
							{
								if(api.Token == null)
								{
									Thread.Sleep(4000);
								}
								var comment = await api.Photo.GetCommentsAsync(new PhotoGetCommentsParams()
								{
									OwnerId = photo.OwnerId,
									PhotoId = (ulong)photo.Id,
									Count = 1,
								});
								Thread.Sleep(150);
								weapon = new WeaponList()
								{
									Text = comment.First().Text,
									PhotoId = (long)photo.Id,
									AlbumId = (long)photo.AlbumId,
									GroupId = (long)photo.OwnerId,
									Src = photo.Sizes.OrderByDescending(w => w.Height).First().Src.ToString(),
									StartTime = Convert.ToDateTime(photo.CreateTime)
								};
								
							}
							catch (Exception e)
							{
								await api.LogOutAsync();
								Thread.Sleep(3000);
								


								if (active_user == userList.Count - 1)
								{
									active_user = 0;
								}
								else
								{
									active_user++;
								}


								await api.AuthorizeAsync(new ApiAuthParams()
								{
									AccessToken = userList[active_user]
								});

								var comment = await api.Photo.GetCommentsAsync(new PhotoGetCommentsParams()
								{
									OwnerId = photo.OwnerId,
									PhotoId = (ulong)photo.Id,
									Count = 1,
								});
								Thread.Sleep(150);
								weapon = new WeaponList()
								{
									Text = comment.First().Text,
									PhotoId = (long)photo.Id,
									AlbumId = (long)photo.AlbumId,
									GroupId = (long)photo.OwnerId,
									Src = photo.Sizes.OrderByDescending(w => w.Height).First().Src.ToString(),
									StartTime = Convert.ToDateTime(photo.CreateTime)
								};

							}

							addList.Add(weapon);

							using (var dbLinq = new DbNorthwind())
							{
								if (photo == photosWithComments.Last())
								{
									dbLinq.BulkCopy(addList);
									addList = new List<WeaponList>();
								}

								if (addList.Count == 10)
								{
									dbLinq.BulkCopy(addList);
									addList = new List<WeaponList>();
								}
							}
							

						}




						var photosList = photos
								.Where(w => w.Text != "");

						photosList = photos
							.Where(w => weaponList.FirstOrDefault(f =>
											f.GroupId == w.OwnerId && f.AlbumId == w.AlbumId && f.PhotoId == w.Id) == null && w.Text != "");

						var addPhoto = new List<WeaponList>();
						foreach (var ph in photosList)
						{
							addPhoto.Add(new WeaponList()
							{
								Text = ph.Text,
								PhotoId = (long)ph.Id,
								AlbumId = (long)ph.AlbumId,
								GroupId = (long)ph.OwnerId,
								Src = ph.Sizes.OrderByDescending(w => w.Height).First().Src.ToString(),
								StartTime = Convert.ToDateTime(ph.CreateTime)
							});
						}
						using (var dbLinq = new DbNorthwind())
						{
							dbLinq.BulkCopy(addPhoto);
						}

					var removeList = weaponList.Where(w => w.GroupId == group.GroupId
										&& w.AlbumId == group.AlbumId
										&& photos.FirstOrDefault(f => f.Id == w.PhotoId) == null).ToList();

					await db.WeaponList.Where(w => removeList.FirstOrDefault(w => w.PhotoId == w.PhotoId).PhotoId == w.PhotoId).DeleteAsync();

						    
					

			    }
		    }



	    }

		public static async Task Test()
		{
			Console.WriteLine("123123");
		}
    }
}