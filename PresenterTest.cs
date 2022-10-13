using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using static NUnit.Framework.Assert;
using static sea_boy.Presenter;

namespace sea_boy;

public class PresenterTest
{
    [Test]
    public void IsShipDeadTest1()
    {
        var board = new CellState[5,5]{
            { CellState.Unknown, CellState.Unknown, CellState.Unknown, CellState.Unknown, CellState.Unknown },
            { CellState.Unknown, CellState.Hit, CellState.Unknown, CellState.Unknown, CellState.Unknown },
            { CellState.Unknown, CellState.Unknown, CellState.Unknown, CellState.Unknown, CellState.Unknown },
            { CellState.Unknown, CellState.Unknown, CellState.Unknown, CellState.Unknown, CellState.Unknown },
            { CellState.Unknown, CellState.Unknown, CellState.Unknown, CellState.Unknown, CellState.Unknown }
        };
        var ship = new BattleShip(ShipType.s1x3, 1, 1);
        That(Presenter.IsShipDead(ship, board), Is.False);
    }

    [Test]
    public void IsShipDeadTest2()
    {
        var board = new CellState[5, 5]{
            { CellState.Unknown, CellState.Unknown, CellState.Unknown, CellState.Unknown, CellState.Unknown },
            { CellState.Unknown, CellState.Hit, CellState.Unknown, CellState.Unknown, CellState.Unknown },
            { CellState.Unknown, CellState.Hit, CellState.Unknown, CellState.Unknown, CellState.Unknown },
            { CellState.Unknown, CellState.Hit, CellState.Unknown, CellState.Unknown, CellState.Unknown },
            { CellState.Unknown, CellState.Unknown, CellState.Unknown, CellState.Unknown, CellState.Unknown }
        };
        var ship = new BattleShip(ShipType.s1x3, 1, 1);
        That(Presenter.IsShipDead(ship, board), Is.True);
    }
}
