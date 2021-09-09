using VoxelEngine.Graphics;

namespace VoxelEngine.Renderer
{
    public class SkyBoxMesh : RenderMesh
    {
        /// <summary>
        /// Буфер точки, точка xyz и цвет точки rgba
        /// </summary>
        protected override int[] _Attrs { get; } = new int[] { 3 };

        public SkyBoxMesh(float k)
        {
            for (int i = 0; i < skyboxVertices.Length; i++) skyboxVertices[i] *= k;
                //for (int i = 0; i < skyboxVertices.Length / 3; i++)
                //{
                //    vec3 v1 = new vec3(skyboxVertices[i * 3], skyboxVertices[i * 3 + 1], skyboxVertices[i * 3 + 2]);
                //    vec3 v2 = v1.rotatePitch(-2.57f);
                //    skyboxVertices[i * 3] = v2.x;
                //    skyboxVertices[i * 3 + 1] = v2.y;
                //    skyboxVertices[i * 3 + 2] = v2.z;
                //}

                Render(skyboxVertices);
        }

        protected float[] skyboxVertices = new float[] {
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f, // солнце
            -1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            -1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f,  1.0f
        };

        //protected float[] skyboxVertices2 = new float[] {
        //    -1.0f,  1.0f, -1.0f,
        //    -1.0f, -1.0f, -1.0f,
        //     1.0f, -1.0f, -1.0f,
        //     1.0f, -1.0f, -1.0f,
        //     1.0f,  1.0f, -1.0f,
        //    -1.0f,  1.0f, -1.0f,

        //    -1.0f, -1.0f,  1.0f,
        //    -1.0f, -1.0f, -1.0f,
        //    -1.0f,  1.0f, -1.0f,
        //    -1.0f,  1.0f, -1.0f,
        //    -1.0f,  1.0f,  1.0f,
        //    -1.0f, -1.0f,  1.0f,

        //     1.0f, -1.0f, -1.0f,
        //     1.0f, -1.0f,  1.0f,
        //     1.0f,  1.0f,  1.0f,
        //     1.0f,  1.0f,  1.0f,
        //     1.0f,  1.0f, -1.0f,
        //     1.0f, -1.0f, -1.0f,

        //    -1.0f, -1.0f,  1.0f,
        //    -1.0f,  1.0f,  1.0f,
        //     1.0f,  1.0f,  1.0f,
        //     1.0f,  1.0f,  1.0f,
        //     1.0f, -1.0f,  1.0f,
        //    -1.0f, -1.0f,  1.0f,

        //    -1.0f,  0.9f, -1.0f,
        //     1.0f,  0.9f, -1.0f,
        //     1.0f,  0.9f,  1.0f,
        //     1.0f,  0.9f,  1.0f,
        //    -1.0f,  0.9f,  1.0f,
        //    -1.0f,  0.9f, -1.0f,

        //    -1.0f, -1.0f, -1.0f,
        //    -1.0f, -1.0f,  1.0f,
        //     1.0f, -1.0f, -1.0f,
        //     1.0f, -1.0f, -1.0f,
        //    -1.0f, -1.0f,  1.0f,
        //     1.0f, -1.0f,  1.0f
        //};
    }
}
