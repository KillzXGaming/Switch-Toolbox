using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstPlugin.Turbo.CourseMuuntStructs;
using FirstPlugin.Turbo;

namespace FirstPlugin.MuuntEditor
{
    public class TrackMuuntLoader : IMuuntLoader
    {
        public List<ObjectGroup> Groups { get; set; }

        public bool Identify(dynamic byml, string fileName)
        {
            return fileName.Contains("_muunt");
        }

        public void Load(dynamic byml)
        {
            var courseMuunt = new CourseMuuntScene(byml);

            Groups = new List<ObjectGroup>();
            Groups.Add(new ObjectGroup("Scene Properties", new PropertyObject()
            {
                Name = "",
                Prop = (Dictionary<string, dynamic>)byml,
            }));

            PathDrawableContainer lapPaths = new PathDrawableContainer("Lap Paths");
            PathDrawableContainer enemyPaths = new PathDrawableContainer("Enemy Paths");
            Groups.Add(lapPaths);
            Groups.Add(enemyPaths);

            for (int i = 0; i < courseMuunt.LapPaths.Count; i++)
            {
                courseMuunt.LapPaths[i].Name = $"Group [{i}]";
                lapPaths.Objects.Add(courseMuunt.LapPaths[i]);
            }

            for  (int i = 0; i < courseMuunt.EnemyPaths.Count; i++)
            {
                courseMuunt.EnemyPaths[i].Name = $"Group [{i}]";
                enemyPaths.Objects.Add(courseMuunt.EnemyPaths[i]);
            }

        }
    }
}
