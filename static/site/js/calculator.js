(function() {
  var Calculator, Mortgage;

  Mortgage = {
    Notional: m.prop(),
    Rate: m.prop(),
    Months: m.prop(),
    SpeedType: m.prop(),
    SpeedAmt: m.prop(),
    Price: m.prop()
  };

  Calculator = {
    view: function(ctrl) {
      return m('div', [m('p', 'test')]);
    }
  };

  m.mount(document.getElementById("calculator"), Calculator);

}).call(this);
