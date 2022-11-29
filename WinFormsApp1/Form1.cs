using System;
using WinFormsApp1.Objects;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        MyRectangle myRect; // �������� ���� ��� ��� �������������
        List<BaseObject> objects = new();
        Player player;
        Marker marker;
        public Form1()
        {
            InitializeComponent();

            player = new Player(pbMain.Width / 2, pbMain.Height / 2, 0);
            player.OnOverlap += (p, obj) =>
            {
                txtLog.Text = $"[{DateTime.Now:HH:mm:ss:ff}] ����� ��������� � {obj}\n" + txtLog.Text;
            };

            // ������� ������� �� ����������� � ��������
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

            updatePlayer();//���������� ������ 

            foreach (var obj in objects.ToList())//������������� �����������
            {
                if (obj != player && player.Overlaps(obj, g))
                {
                    player.Overlap(obj); // �� ���� ����� ��������� � ��������
                    obj.Overlap(player); // � ������ ��������� � �������

                }
            }
            // �������� �������
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

                // �� ���� �� ������ ���������� ������ dx, dy
                // ��� ������ ���������, ������ ���� ������ ����������
                // ������� ����������� ������ � �������
                // 0.5 ������ ����������� ������� �������� �� ����
                // � ������� ���� ������������ �������� ��������
                player.vX += dx * 0.5f;
                player.vY += dy * 0.5f;

                // ����������� ���� �������� ������ 
                player.Angle = 90 - MathF.Atan2(player.vX, player.vY) * 180 / MathF.PI;
            }

            // ���������� ������,
            // ����� �����, ����� ����� ��������� ������� ��������� ����������� ����������
            player.vX += -player.vX * 0.1f;
            player.vY += -player.vY * 0.1f;

            // �������� ������� ������ � ������� ������� ��������
            player.X += player.vX;
            player.Y += player.vY;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            // ����������� ���������� pbMain
            // ��� ������� ����� pbMain_Paint �� �����
            pbMain.Invalidate();
        }

        private void pbMain_MouseClick(object sender, MouseEventArgs e)
        {
            // ��� ������� �������� ������� �� ����� ���� �� ��� �� ������
            if (marker == null)
            {
                marker = new Marker(0, 0, 0);
                objects.Add(marker); // � ������� �� ������ ��������� � objects
            }

            // � ��� ��� � ��������
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