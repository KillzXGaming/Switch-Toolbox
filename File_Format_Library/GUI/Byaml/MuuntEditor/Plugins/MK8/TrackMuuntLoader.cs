using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
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

            var pathGroup = new ObjectGroup("Scene Objects");
            Groups.Add(pathGroup);
            PathDoublePointDrawableContainer lapPaths = new PathDoublePointDrawableContainer("Lap Paths", Color.Cyan);
            PathDrawableContainer enemyPaths = new PathDrawableContainer("Enemy Paths", Color.Red);
            PathDrawableContainer glidePaths = new PathDrawableContainer("Glide Paths", Color.Orange);
            PathDrawableContainer itemPaths = new PathDrawableContainer("Item Paths", Color.Green);
            PathDrawableContainer steerAssitPaths = new PathDrawableContainer("Steer Assist Paths", Color.Crimson);
            PathDrawableContainer gravityPaths = new PathDrawableContainer("Gravity Paths", Color.Purple);
            PathDrawableContainer pullPaths = new PathDrawableContainer("Pull Paths", Color.DarkSlateGray);
            PathDrawableContainer paths = new PathDrawableContainer("Paths", Color.GreenYellow);
            PathDrawableContainer objPaths = new PathDrawableContainer("Object Paths", Color.Brown);
            PathDrawableContainer jugemPaths = new PathDrawableContainer("Latiku Paths", Color.Pink);
            PathDrawableContainer introCameras = new PathDrawableContainer("Intro Camera", Color.Yellow);

            pathGroup.Objects.Add(lapPaths);
            pathGroup.Objects.Add(enemyPaths);
            pathGroup.Objects.Add(glidePaths);
            pathGroup.Objects.Add(itemPaths);
            pathGroup.Objects.Add(steerAssitPaths);
            pathGroup.Objects.Add(gravityPaths);
            pathGroup.Objects.Add(pullPaths);
            pathGroup.Objects.Add(paths);
            pathGroup.Objects.Add(objPaths);
            pathGroup.Objects.Add(jugemPaths);
            pathGroup.Objects.Add(introCameras);

            for (int i = 0; i < courseMuunt.LapPaths.Count; i++)
            {
                courseMuunt.LapPaths[i].Name = $"Group [{i}]";
                lapPaths.SubObjects.Add(courseMuunt.LapPaths[i]);
            }

            for  (int i = 0; i < courseMuunt.EnemyPaths.Count; i++)
            {
                courseMuunt.EnemyPaths[i].Name = $"Group [{i}]";
                enemyPaths.SubObjects.Add(courseMuunt.EnemyPaths[i]);
            }
        }
    }
}
