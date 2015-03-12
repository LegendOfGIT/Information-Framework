using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationFramework.Presentation.Modifications
{
    public class Modification
    {
        public int Repetitions { get; set; }
        public bool Active { get; set; }
        public Modification Parent { get; set; }
        public IEnumerable<Modification> Modifications { get; set; }

        public event EventHandler OnLeave;

        public void OnLeaveHandler(object sender) {
            if (OnLeave != null) {
                OnLeave(sender, null);
            }
        }
    }
}
