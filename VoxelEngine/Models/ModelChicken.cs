using System.Collections.Generic;
using VoxelEngine.Entity;
using VoxelEngine.Glm;

namespace VoxelEngine.Models
{
    /// <summary>
    /// Объект 3д модели курочки
    /// </summary>
    public class ModelChicken : ModelBase
    {
        public ModelBox Head { get; protected set; }
        public ModelBox Bill { get; protected set; }
        public ModelBox Chin { get; protected set; }
        public ModelBox Body { get; protected set; }
        public ModelBox RightLeg { get; protected set; }
        public ModelBox LeftLeg { get; protected set; }
        public ModelBox RightWing { get; protected set; }
        public ModelBox LeftWing { get; protected set; }
        public ModelBox Dot { get; protected set; }

        public ModelChicken()
        {
            Head = new ModelBox(this, 0, 0, -2.0f, -6.0f, -2.0f, 4, 6, 3, 0f);
            Head.SetRotationPoint(0, 15, -4);
            Bill = new ModelBox(this, 14, 0, -2.0f, -4.0f, -4.0f, 4, 2, 2, 0f);
            Bill.SetRotationPoint(0, 15, -4);
            Chin = new ModelBox(this, 14, 4, -1.0f, -2.0f, -3.0f, 2, 2, 2, 0f);
            Chin.SetRotationPoint(0, 15, -4);
            Body = new ModelBox(this, 0, 9, -3.0f, -4.0f, -3.0f, 6, 8, 6, 0f);
            Body.SetRotationPoint(0, 16, 0);
            RightLeg = new ModelBox(this, 26, 0, -1.0f, 0.0f, -3.0f, 3, 5, 3, 0f);
            RightLeg.SetRotationPoint(-2, 19, 1);
            LeftLeg = new ModelBox(this, 26, 0, -1.0f, 0.0f, -3.0f, 3, 5, 3, 0f);
            LeftLeg.SetRotationPoint(1, 19, 1);
            RightWing = new ModelBox(this, 24, 13, 0.0f, 0.0f, -3.0f, 1, 4, 6, 0f);
            RightWing.SetRotationPoint(-4, 13, 0);
            LeftWing = new ModelBox(this, 24, 13, -1.0F, 0.0F, -3.0F, 1, 4, 6, 0f);
            LeftWing.SetRotationPoint(4, 13, 0);

            //Dot = new ModelBox(this, 14, 4, 0f, 0f, 0f, 2, 2, 2, 0f);
            //Dot.SetRotationPoint(0, 24, 0);
        }

        public override void Render(EntityBase entity, float limbSwing, float limbSwingAmount, float ageInTicks, float headYaw, float headPitch, float scale)
        {
            SetRotationAngles(entity, limbSwing, limbSwingAmount, ageInTicks, headYaw, headPitch, scale);

            Head.Render(entity.Position, entity.Yaw, scale);
            Bill.Render(entity.Position, entity.Yaw, scale);
            Chin.Render(entity.Position, entity.Yaw, scale);
            Body.Render(entity.Position, entity.Yaw, scale);
            RightLeg.Render(entity.Position, entity.Yaw, scale);
            LeftLeg.Render(entity.Position, entity.Yaw, scale);
            RightWing.Render(entity.Position, entity.Yaw, scale);
            LeftWing.Render(entity.Position, entity.Yaw, scale);
            //Dot.Render(entity.Position, entity.Yaw, scale);

            List<float> buffer = new List<float>();
            buffer.AddRange(Head.Buffer);
            buffer.AddRange(Bill.Buffer);
            buffer.AddRange(Chin.Buffer);
            buffer.AddRange(Body.Buffer);
            buffer.AddRange(RightLeg.Buffer);
            buffer.AddRange(LeftLeg.Buffer);
            buffer.AddRange(RightWing.Buffer);
            buffer.AddRange(LeftWing.Buffer);
            //buffer.AddRange(Dot.Buffer);
            Buffer = buffer.ToArray();
        }

        public override void SetRotationAngles(EntityBase entity, float limbSwing, 
            float limbSwingAmount, float ageInTicks, float headYaw, float headPitch, float scale)
        {
            Head.RotateAngleX = headPitch / (180f / glm.pi);
            Head.RotateAngleY = headYaw / (180f / glm.pi);
            //Head.RotateAngleX = (float)System.Math.Cos(ageInTicks * 0.6662f);// glm.cos(ageInTicks);// glm.pi90;
            //Head.RotateAngleX = glm.cos(t);// glm.cos(ageInTicks);// glm.pi90;
            //Head.RotateAngleY = glm.pi45;
            Bill.RotateAngleX = Head.RotateAngleX;
            Bill.RotateAngleY = Head.RotateAngleY;
            Chin.RotateAngleX = Head.RotateAngleX;
            Chin.RotateAngleY = Head.RotateAngleY;
            Body.RotateAngleX = glm.pi90;
            RightLeg.RotateAngleX = glm.cos(limbSwing * 0.6662f) * 1.4f * limbSwingAmount;
            LeftLeg.RotateAngleX = glm.cos(limbSwing * 0.6662f + glm.pi) * 1.4f * limbSwingAmount;
            //RightWing.RotateAngleZ = glm.cos(limbSwing * 0.16662f) + glm.pi45;
            //LeftWing.RotateAngleZ = glm.cos(limbSwing * 0.6662f + glm.pi) - glm.pi45;
            //RightWing.RotateAngleZ = ageInTicks;
            //LeftWing.RotateAngleZ = -ageInTicks;
        }
    }
}
