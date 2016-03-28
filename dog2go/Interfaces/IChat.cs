using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;

namespace dog2go.Interfaces
{
    internal interface IChat : IHub
    {
        void SendMessage(string message);
        void SendMessageTo(string message, User recipient);
        string ReceiveMessage(User user);
    }

    interface IGamingTable : IHub
    {
        //void SendTableFields(List<Field> fields);
        void SendGameTable(List<PlayerFieldArea> areas);
    }

    interface IMoveValidation : IHub
    {
        bool validateMove(MeepleMove meetlemove); // valid?
    }

    internal class MeepleMove
    {
        Meeple _meeple;
        MoveDestinationField _moveDestination;
    }

    internal abstract class MoveDestinationField
    {
        int _identifier; // generate identifier on run time (UUID)
        MoveDestinationField _previous;
        MoveDestinationField _next;
    }

    internal class KennelField : MoveDestinationField
    {
        
    }

    internal class StartField : MoveDestinationField
    {
        bool _blocked;
    }

    internal class EndField : MoveDestinationField
    {
        
    }

    internal class StandardField : MoveDestinationField
    {
        
    }

    internal class PlayerFieldArea
    {
        string _colorCode;
        List<MoveDestinationField> _fields;
        List<Meeple> _meeples;
        Participation _participation;
        PlayerFieldArea previous;
        PlayerFieldArea next;
    }

    internal class Participation
    {
        User participant;
        User partner;
        GameTable table;
    }

    internal class GameTable
    {
        string name;
        int identifier;
        DateTime start;
        DateTime? end;
    }

    internal class Meeple
    {
        string _colorCode;
        MoveDestinationField _currentPosition;
    }


    internal class User
    {
        int _identifier;
        string _nickname;
    }
}
