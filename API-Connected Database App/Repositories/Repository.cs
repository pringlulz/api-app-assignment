using API_Connected_Database_App.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Connected_Database_App.Repositories
{
    public class Repository
    {
        SQLiteAsyncConnection? database;

        async Task Init()
        {
            System.Diagnostics.Debug.WriteLine("~~~~~~~~~ initing database......... ~~~~~~~~~~~");
            if (database is not null)
                return;
            database = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "PostSQLite.db3"), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            var result = await database.CreateTableAsync<Models.Post>();
            System.Diagnostics.Debug.WriteLine("~~~~~~~~~ init'd database ~~~~~~~~~~~");
        }

        public async Task<int> SaveItemAsync(Post post)
        {
            await Init();
            var result = await database.UpdateAsync(post); //try update
            if (result == 0) //if no records updated
            {
                result = await database.InsertAsync(post); //try insert
            }
            return result;
        }

        public async Task<int> SavePostsAsync(List<Post> list)
        {
            await Init();
            var result = await database.UpdateAllAsync(list);
            if (result != list.Count)
            {
                result = await database.InsertAllAsync(list);
            }
            return result;
        }

        public async Task<List<Models.Post>> GetPostsAsync()
        {
            await Init();
            return await database.Table<Models.Post>().ToListAsync();
        }


        //public async Task<Models.Post> GetPostAsync(int id)
        //{

        //}

    }
}
