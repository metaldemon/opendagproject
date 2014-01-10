using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opendagproject.Game.Particles
{
    class ParticleHandler
    {

        public static List<Particle> particleGameList = new List<Particle>();
        public static List<Particle> particleList = new List<Particle>();

        public static void initialize()
        {
            particleList.ForEach(x => x.init());
        }

        public static void tick()
        {
            if (particleGameList.Count > 1)
            {

            }
            particleGameList = particleGameList.Where(x => !x.remove).ToList();
            foreach (var x in particleGameList)
            {
                x.tick();
            }
        }

        public static void draw()
        {
            foreach (var x in particleGameList)
            {
                x.draw();
            }
        }
    }
}
