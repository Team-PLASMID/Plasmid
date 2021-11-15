using System;
using Microsoft.Xna.Framework;

namespace Plasmid.Graphics
{
    public class Camera
    {
        private static float minZ = 1f;
        private static float maxZ = 2048f;
        private static int minZoom = 1;
        private static int maxZoom = 20;
        public static float MinZ { get { return minZ; } }
        public static float MaxZ { get { return maxZ; } }
        public static int MinZoom { get { return minZoom; } }
        public static int MaxZoom { get { return maxZoom; } }

        private Vector2 position;
        private float z;
        private int zoom;
        public float BaseZ { get; }

        private float aspectRatio;
        private float fieldOfView;

        private Matrix view;
        private Matrix proj;

        public Vector2 Position { get { return this.position; } }
        public float Z { get { return this.z; } }
        public Matrix View { get { return this.view; } }
        public Matrix Projection { get { return this.proj; } }

        public Camera(Screen screen)
        {
            if (screen is null)
                throw new ArgumentNullException("screen");

            this.aspectRatio = (float)screen.Width / screen.Height;
            this.fieldOfView = MathHelper.PiOver2;

            this.position = new Vector2(0, 0);
            this.BaseZ = this.GetZFromHeight(screen.Height);
            this.z = this.BaseZ;

            this.UpdateMatrices();

            this.zoom = 1;
        }

        public void SetZLimits(int min, int max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("min is greater than max");

            Camera.minZ = min;
            Camera.maxZ = max;
        }

        public void SetZoomLimits(int min, int max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("min is greater than max");

            Camera.minZoom = min;
            Camera.maxZoom = max;
        }

        public void UpdateMatrices()
        {
            this.view = Matrix.CreateLookAt(new Vector3(0, 0, this.z), Vector3.Zero, Vector3.Up);
            this.proj = Matrix.CreatePerspectiveFieldOfView(this.fieldOfView, this.aspectRatio, MinZ, MaxZ);
        }

        public float GetZFromHeight(float height)
        {
            return (0.5f * height) / MathF.Tan(0.5f * this.fieldOfView);
        }

        public float GetHeightFromZ()
        {
            return this.z * MathF.Tan(0.5f * this.fieldOfView) * 2f;
        }

        public void GetExtents(out float width, out float height)
        {
            height = this.GetHeightFromZ();
            width = height * this.aspectRatio;
        }

        public void GetExtents(out float left, out float right, out float bottom, out float top)
        {
            this.GetExtents(out float width, out float height);
            left = this.position.X - width * 0.5f;
            right = left + width;
            bottom = this.position.Y - height * 0.5f;
            top = bottom + height;
        }

        public void GetExtents(out Vector2 min, out Vector2 max)
        {
            this.GetExtents(out float left, out float right, out float bottom, out float top);
            min = new Vector2(left, bottom);
            max = new Vector2(right, top);
        }

        public void MoveZ(float amount)
        {
            this.z += amount;
            this.z = GraphUtils.Clamp(this.z, Camera.MinZ, Camera.MaxZ);
        }

        public void ResetZ()
        {
            this.z = this.BaseZ;
        }

        public void Move(Vector2 amount)
        {
            this.position += amount;
        }

        public void MoveTo(Vector2 position)
        {
            this.position = position;
        }

        public void IncZoom()
        {
            this.zoom++;
            this.zoom = GraphUtils.Clamp(this.zoom, Camera.MinZoom, Camera.MaxZoom);
            this.z = this.BaseZ / this.zoom;
        }
        public void DecZoom()
        {
            this.zoom--;
            this.zoom = GraphUtils.Clamp(this.zoom, Camera.MinZoom, Camera.MaxZoom);
            this.z = this.BaseZ / this.zoom;
        }
        public void SetZoom(int amount)
        {
            this.zoom = GraphUtils.Clamp(amount, Camera.MinZoom, Camera.MaxZoom);
            this.z = this.BaseZ / this.zoom;
        }

    }
}
