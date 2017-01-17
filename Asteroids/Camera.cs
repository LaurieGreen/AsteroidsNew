using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    public class Camera
    {
        //camera class
        protected Vector3 topDown = new Vector3(0.0f, 0.0f, 14.0f);
        protected Vector3 chase = new Vector3(0.0f, 1500.0f, 200.0f);

        public Vector3 Position;
        public Vector3 Target;

        public Matrix World;
        public Matrix View;
        public Matrix Projection;

        float aspectRatio;
        float foView;
        float near;
        float far;

        public Camera(Vector3 target, float aspectratio, float foview)
        {
            Position = topDown;
            Target = target;
            aspectRatio = aspectratio;
            foView = foview;
            near = 0.1f;
            far = 20000f;

            World = Matrix.CreateTranslation(Target);
            View = Matrix.CreateLookAt(Position, Target, Vector3.Up);
            //Projection = Matrix.CreatePerspectiveFieldOfView(foView, aspectRatio, near, far);
            Projection = Matrix.CreateOrthographic(40f, 22.5f, near, far);
        }

        public Matrix[] SetupEffectDefaults(Model model, Camera camera)
        {
            Matrix[] absoluteTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = camera.Projection;
                    effect.View = camera.View;
                }
            }
            return absoluteTransforms;
        }

        //used for player and all asteroids
        public void DrawModel(Model model, Matrix modelTransform, Matrix[] absoluteBoneTransforms, Camera camera, Vector3 DiffuseColor)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0, 204, 0); // a red light
                    effect.DirectionalLight0.Direction = new Vector3(1, 1, 0);  // coming along the x-axis
                    effect.DirectionalLight0.SpecularColor = new Vector3(0, 0, 0); // with green highlights
                    
                    effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
                    effect.EmissiveColor = new Vector3(0.5f, 0.5f, 0.5f);

                    effect.FogEnabled = true;
                    effect.FogColor = Color.Black.ToVector3(); // For best results, make this color whatever your background is.
                    effect.FogStart = 9.75f;
                    effect.FogEnd = 16.25f;

                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        public void DrawParticle(Model model, Camera camera, Matrix world)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.DirectionalLight0.DiffuseColor = new Vector3(178, 34, 34); 
                    effect.DirectionalLight0.Direction = new Vector3(0, 0, 1);  // coming along the y-axis
                    effect.DirectionalLight0.SpecularColor = new Vector3(178, 34, 34);

                    effect.AmbientLightColor = new Vector3(0, 0, 0);
                    effect.EmissiveColor = new Vector3(0, 0, 0);

                    effect.FogEnabled = true;
                    effect.FogColor = Color.DarkOrange.ToVector3(); // For best results, make this color whatever your background is.
                    effect.FogStart = 9.75f;
                    effect.FogEnd = 15.25f;

                    effect.World = world;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        public void DrawBullet(Model model, Camera camera, Matrix world)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0, 0, 0); // a red light
                    effect.DirectionalLight0.Direction = new Vector3(0, 0, 0);  // coming along the x-axis
                    effect.DirectionalLight0.SpecularColor = new Vector3(0, 0, 0); // with green highlights

                    effect.AmbientLightColor = new Vector3(0, 0, 0);
                    effect.EmissiveColor = new Vector3(0, 0, 0);

                    effect.FogEnabled = true;
                    effect.FogColor = Color.LimeGreen.ToVector3(); // For best results, make this color whatever your background is.
                    effect.FogStart = 9.75f;
                    effect.FogEnd = 15.25f;

                    effect.World = world;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        public void Draw()
        {

        }

    }
}
