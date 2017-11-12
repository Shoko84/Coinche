using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /**
     * This class is sent when the player is renamed
     */
    public class PlayerRename
    {
        public int Id;          /**< This int corresponds to the user's id*/
        public string OldName;  /**< This string corresponds to the old user's name*/
        public string NewName;  /**< This string corresponds to the new user's name*/

        /**
         *  Constructor of PlayerRename class
         *  @param  id          The user's id
         *  @param  oldName     The old user's name.
         *  @param  newName     The new user's name.
         */
        public PlayerRename(int id, string oldName, string newName)
        {
            Id = id;
            OldName = oldName;
            NewName = newName;
        }
    }
}
