namespace VoxelEngine.Renderer.Entity
{

    public class EntityRender
    {
        #region Event

        protected void HitBox_Done(object sender, BufferEventArgs e)
        {
            OnHitBoxDone(e);
        }

        /// <summary>
        /// Событие чанк сделано
        /// </summary>
        public event BufferEventHandler HitBoxDone;
        /// <summary>
        /// Событие чанк сделано
        /// </summary>
        protected void OnHitBoxDone(BufferEventArgs e)
        {
            HitBoxDone?.Invoke(this, e);
        }

        #endregion
    }
}
