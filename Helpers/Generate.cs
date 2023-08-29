using Gamanet.C4.SimpleInterfaces;
using System.Collections.Generic;
using System;

namespace WpfApp1.Helpers
{
    public class Generate
    {
        public List<SimpleEntity> Tree(int nodesOnOneLevel, int nodesOnSecondLevel, int depth)
        {
            var rand = new Random();
            var list = new List<SimpleEntity>();
            var root = CreateItem(Guid.Empty, EntityCategory.PersonSuperRoot, 0);
            root.Id = EntityRoot.PersonSuperRoot;
            list.Add(root);
            var currentDepthLevel = 0;

            MoveToNextLevel(root.Id, 4, 2, 3, ++currentDepthLevel, list, rand);
            //MoveToNextLevel(root.Id, nodesOnOneLevel, nodesOnSecondLevel, depth, ++currentDepthLevel, list, rand);

            return list;
        }

        //public List<SimpleEntity> GetAll()
        //{
        //    // get data
        //    var rand = new Random();
        //    var list = new List<SimpleEntity>();
        //    var root = CreateItem(Guid.Empty, EntityCategory.PersonSuperRoot, 0);
        //    root.Id = EntityRoot.PersonSuperRoot;
        //    list.Add(root);

        //    for (int i = 0; i < 255; i++)
        //    {
        //        var categoryId = _categories[rand.Next(0, 7)];
        //        var item = CreateItem(root.Id, categoryId, i);
        //        list.Add(item);

        //        for (int j = 0; j < 8; j++)
        //        {
        //            categoryId = _categories[rand.Next(0, 7)];
        //            var child = CreateItem(item.Id, categoryId, j);
        //            list.Add(child);

        //            for (int k = 0; k < 555; k++)
        //            {
        //                categoryId = _categories[rand.Next(0, 7)];
        //                child = CreateItem(item.Id, categoryId, j);
        //                list.Add(child);
        //            }
        //        }
        //    }

        //    return list;
        //}

        private void MoveToNextLevel(Guid parentId, int nodesOnOneLevel, int nodesOnSecondLevel, int depth, int currentDepthLevel, List<SimpleEntity> list, Random rand)
        {
            if (currentDepthLevel == depth)
                return;

            if (currentDepthLevel == 2)
            {
                for (int i = 1; i < nodesOnSecondLevel; i++)
                {
                    var categoryId = _categories[rand.Next(0, 7)];
                    var item = CreateItem(parentId, categoryId, i);
                    list.Add(item);

                    var nextDepthLevel = currentDepthLevel + 1;
                    MoveToNextLevel(item.Id, nodesOnOneLevel, nodesOnSecondLevel, depth, nextDepthLevel, list, rand);
                }
            }
            else
            {
                for (int i = 1; i < nodesOnOneLevel; i++)
                {
                    var categoryId = _categories[rand.Next(0, 7)];
                    var item = CreateItem(parentId, categoryId, i);
                    list.Add(item);

                    var nextDepthLevel = currentDepthLevel + 1;
                    MoveToNextLevel(item.Id, nodesOnOneLevel, nodesOnSecondLevel, depth, nextDepthLevel, list, rand);
                }
            }
        }

        private SimpleEntity CreateItem(Guid parentId, Guid categoryId, int index)
        {
            var id = Guid.NewGuid();
            var simplePerson = new SimplePersonV1(id, parentId, categoryId)
            {
                Name = $"Name {index}",
            };

            return simplePerson;
        }

        private Dictionary<int, Guid> _categories
          = new Dictionary<int, Guid>
          {
              {0, Guid.Parse("5FEFA0A9-70EE-46AA-BBBD-BED5888C445C") }, // person
              {1, Guid.Parse("47336057-7ECB-4D0B-9E82-48BE62556D95") }, // company
              {2, Guid.Parse("8289478C-D06D-46D4-ADDE-729FD7A96798") }, // department
              {3, Guid.Parse("F9823EA2-D234-444C-B32D-7D45DBB5AB03") }, // division
              {4, Guid.Parse("1346E412-213E-4553-807A-7EE8C0944FD5") }, // center
              {5, Guid.Parse("F26D0796-A751-4B78-A6ED-C154CBEC602B") }, // vehicle
              {6, Guid.Parse("4DB7ED74-D567-478F-95F7-F6AADD9833E9") }, // manager
              {7, Guid.Parse("AE1E65E5-1124-45D8-A6CF-930B9D96EB5E") }, // external
          };
    }
}
