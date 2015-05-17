using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban.Logic
{
    public class Soko : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        //
        public enum MoveDirection
        {
            Up,
            Down,
            Right,
            Left
        }
    }
}
