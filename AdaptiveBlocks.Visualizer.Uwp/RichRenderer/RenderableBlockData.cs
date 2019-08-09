
using AdaptiveBlocks.Visualizer.Uwp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Visualizer.Uwp.RichRenderer
{
    public class RenderableBlockData : BindableBase
    {
        public RenderableBlockData(AdaptiveBlock blockSource)
        {
            BlockSource = blockSource;
            Block = blockSource?.View?.Content;
            Reset();
        }

        public AdaptiveBlock BlockSource { get; set; }
        public AdaptiveBlockContent Block { get; set; }

        private bool m_isColumn;
        public bool IsColumn
        {
            get { return m_isColumn; }
            set { SetProperty(ref m_isColumn, value); }
        }

        private bool m_isFactSet;
        public bool IsFactSet
        {
            get { return m_isFactSet; }
            set { SetProperty(ref m_isFactSet, value); }
        }

        private bool m_isDropped = true;
        public bool IsDropped
        {
            get { return m_isDropped; }
            set { SetProperty(ref m_isDropped, value); }
        }

        public List<RenderableBlockData> Children { get; private set; } = new List<RenderableBlockData>();

        public RenderableBlockData Parent { get; set; }

        public int GetLevelsDeep()
        {
            if (Parent == null)
            {
                return 0;
            }

            return 1 + Parent.GetLevelsDeep();
        }

        public void Reset()
        {
            IsColumn = BlockSource.Hints.Column;
            IsFactSet = BlockSource.Hints.FactSet;
            IsDropped = true;
        }
    }
}
