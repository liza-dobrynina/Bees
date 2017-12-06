using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace bees
{

    public interface IWorldOptions
    {
        List<string> NamesBees { get; set; }
        Dictionary<string, Color> ColorObjects { get; set; }
        double HungerProgress { get; set; } // скорость голодания
        int StartFullness { get; set; } // начальная сытость
        double WorldWidth { get; set; } //ширина мира
        double WorldHeight { get; set; } //высота мира
        double RenewalFlowes { get; set; } //скорость обновления цветка
        double BeesSpeed { get; set; } //скорость пчел
        float TimeTick { get; set; } //внутренние часы
        int MaxStoredFoodCount { get; set; } // сколько максимум цветов может помнить пчела
        double VisionDistance { get; set; }     // расстояние, на которое пчела видит
        int LowFoodTreshold { get; set; } // нижний порог для еды
        int CountEatInBeehive { get; set; } //поличество еды в улье
    }

    // интерфейс "мира"
    public interface IWorld : IActive, IDrawable
    {
        IWorldOptions WorldOptions { get; }
        IList<T> FindObjectOnDistance<T>(double X, double Y, double distance) where T : IWorldObject;
        void AddObject(IWorldObject obj);
        float CurrentTime { get; set; } //текущее время
    }

    // интерфейс "активного" объекта
    public interface IActive
    {
        void Action();
    }

    
    public interface IPosition
    {
        double X { get; }
        double Y { get; }
        double GetDistance(double X, double Y);
    }


    public interface IDrawable
    {
        void Draw(Graphics g);
    }



    public interface IWorldObject : IActive, IPosition, IDrawable
    {
        bool Living { get; set; }
    }



    public class WorldOptions : IWorldOptions
    {
        public List<string> NamesBees { get; set; }
        public Dictionary<string, Color> ColorObjects { get; set; }
        public double HungerProgress { get; set; } // скорость голодания
        public int StartFullness { get; set; } // начальная сытость
        public double WorldWidth { get; set; } //ширина мира
        public double WorldHeight { get; set; } //высота мира
        public double RenewalFlowes { get; set; } //скорость обновления цветка
        public double BeesSpeed { get; set; } //скорость пчел
        public float TimeTick { get; set; } //внутренние часы
        public int MaxStoredFoodCount { get; set; } // сколько максимум цветов может помнить пчела
        public double VisionDistance { get; set; }     // расстояние, на которое пчела видит
        public int LowFoodTreshold { get; set; } // нижний порог для еды
        public int CountEatInBeehive { get; set; } //количество еды в улье

        public WorldOptions()
        {
            NamesBees = new List<string> { "Working", "Female", "MaleBee" };
            ColorObjects = new Dictionary<string, Color>
            {
                {"Female", Color.Coral },
                {"Working", Color.Yellow },
                {"MaleBee", Color.Khaki },
                {"Flower", Color.HotPink },
                {"Beehive", Color.SaddleBrown }
            };
            WorldWidth = 600;
            WorldHeight = 400;

            HungerProgress = 0.5;
            StartFullness = 100;

            RenewalFlowes = 0.1;
            MaxStoredFoodCount = 3;
            VisionDistance = 80;
            LowFoodTreshold = 2;
            CountEatInBeehive = 0;
            TimeTick = 0.2f;
            BeesSpeed = 4;
        }
    }



    
    //класс, реализующий "мир"
    public class World : IWorld
    {
       public World(int height, int width) : this()
        {
            WorldOptions.WorldHeight = height;
            WorldOptions.WorldWidth = width;
        }
        public List<IWorldObject> objects = new List<IWorldObject>();

        public IWorldOptions WorldOptions
        {
            get;
            private set;
        }
        private float currentTime;
        public float CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                currentTime = value;
                if (value >= 24)
                    currentTime = 0;
            }
        }

        public World()
        {
            WorldOptions = new WorldOptions();
            CurrentTime = 0;
        }

        public void AddObject(IWorldObject obj)
        {
            objects.Add(obj);
        }

        public void AddObject(string name, double X, double Y)
        {

        }

        public void RemoveObject(IWorldObject obj)
        {
            objects.Remove(obj);
        }

        public void Action()
        {
            int count = objects.Count;
            CurrentTime += WorldOptions.TimeTick;
            for (int i = 0; i < count; i++)
            {
                objects[i].Action();
            }
            for (int i = objects.Count - 1; i > 0; i--)
            {
                if (!objects.ToList()[i].Living)
                    objects.Remove(objects.ToList()[i]);
            }
        }

        public void Draw(Graphics g)
        {
            foreach (IWorldObject obj in objects)
            {
                //g.ResetTransform();
                //g.TranslateTransform(Convert.ToInt32(obj.X), Convert.ToInt32(obj.Y));
                obj.Draw(g);
            }
        }

        public virtual IList<T> FindObjectOnDistance<T>(double X, double Y, double distance) where T : IWorldObject
        {
            IList<T> list = new List<T>();
            foreach (IWorldObject obj in objects)
            {
                if (obj is T && obj.GetDistance(X, Y) < distance)
                {
                    list.Add((T)obj);
                }
            }
            return list;
        }
    }

    // базовый абстрактый класс для всех обитателей мира, реализует общую функциональность
    public abstract class BaseWorldObject : IWorldObject, IPosition, IActive, IDrawable
    {
        public bool Living { get; set; }

        public IWorld World
        {
            get;
            protected set;
        }

        public IWorldOptions WorldOptions
        {
            get
            {
                return World?.WorldOptions;
            }
        }

        public virtual double X
        {
            get;
            protected set;
        }

        public virtual double Y
        {
            get;
            protected set;
        }

        public virtual double GetDistance(double X, double Y)
        {
            double dx = X - this.X,
                   dy = Y - this.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public abstract void Action();

        public abstract void Draw(Graphics g);

        public bool IsNearBy(IPosition obj)
        {
            if (obj != null)
                return (Math.Abs(X - obj.X) < WorldOptions.HumanWidth * 3 && Math.Abs(Y - obj.Y) < WorldOptions.HumanWidth * 3);//0.001
            else
                return false;
        }
        public bool IsNearBy(double targetX, double targetY)
        {
            if (X == targetX && Y == targetY)
                return true;
            return Math.Abs(X - targetX) < WorldOptions.HumanWidth && Math.Abs(Y - targetY) < WorldOptions.HumanWidth;//0.001
        }

        protected BaseWorldObject(IWorld world, double x, double y)
        {
            this.Living = true;
            this.X = x;
            this.Y = y;
            this.World = world;
        }

        protected BaseWorldObject(IWorld world)
          : this(world, 0, 0)
        {
            this.Living = true;
        }

        
    }
}