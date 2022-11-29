using System;
using WinFormsApp1.Objects;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        MyRectangle myRect; // создадим поле под наш пр€моугольник
        List<BaseObject> objects = new();
        Player player;
        Marker marker;
        public Form1()
        {
            InitializeComponent();

            player = new Player(pbMain.Width / 2, pbMain.Height / 2, 0);
            player.OnOverlap += (p, obj) =>
            {
                txtLog.Text = $"[{DateTime.Now:HH:mm:ss:ff}] »грок пересекс€ с {obj}\n" + txtLog.Text;
            };

            // добавил реакцию на пересечение с маркером
            player.OnMarkerOverlap += (m) =>
            {
                objects.Remove(m);
                marker = null;
            };

            marker = new Marker(pbMain.Width / 2+ 50, pbMain.Height / 2 + 50, 0);

            objects.Add(marker);

            objects.Add(player);
            objects.Add(new MyRectangle(50, 50, 0));
            objects.Add(new MyRectangle(100, 100, 45));
        }
        private void pbMain_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            g.Clear(Color.White);

            updatePlayer();//перерасчЄт игрока 

            foreach (var obj in objects.ToList())//пересчитываем пересечени€
            {
                if (obj != player && player.Overlaps(obj, g))
                {
                    player.Overlap(obj); // то есть игрок пересекс€ с объектом
                    obj.Overlap(player); // и объект пересекс€ с игроком

                }
            }
            // рендерим объекты
            foreach (var obj in objects)
            {
                g.Transform = obj.GetTransform();
                obj.Render(g);
            }
        }

        private void updatePlayer()
        {
            if (marker != null)
            {
                float dx = marker.X - player.X;
                float dy = marker.Y - player.Y;
                float length = MathF.Sqrt(dx * dx + dy * dy);
                dx /= length;
                dy /= length;

                // по сути мы теперь используем вектор dx, dy
                // как вектор ускорени€, точнее даже вектор прит€жени€
                // который прит€гивает игрока к маркеру
                // 0.5 просто коэффициент который подобрал на глаз
                // и который дает естественное ощущение движени€
                player.vX += dx * 0.5f;
                player.vY += dy * 0.5f;

                // расчитываем угол поворота игрока 
                player.Angle = 90 - MathF.Atan2(player.vX, player.vY) * 180 / MathF.PI;
            }

            // тормоз€щий момент,
            // нужен чтобы, когда игрок достигнет маркера произошло постепенное замедление
            player.vX += -player.vX * 0.1f;
            player.vY += -player.vY * 0.1f;

            // пересчет позици€ игрока с помощью вектора скорости
            player.X += player.vX;
            player.Y += player.vY;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            // запрашиваем обновление pbMain
            // это вызовет метод pbMain_Paint по новой
            pbMain.Invalidate();
        }

        private void pbMain_MouseClick(object sender, MouseEventArgs e)
        {
            // тут добавил создание маркера по клику если он еще не создан
            if (marker == null)
            {
                marker = new Marker(0, 0, 0);
                objects.Add(marker); // и главное не забыть пололжить в objects
            }

            // а это так и остаетс€
            marker.X = e.X;
            marker.Y = e.Y;
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}