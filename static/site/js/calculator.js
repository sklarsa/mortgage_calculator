(function() {
  var Calculator, Mortgage;

  Mortgage = {
    Notional: m.prop(),
    Rate: m.prop(),
    Months: m.prop(),
    SpeedType: m.prop(),
    SpeedAmt: m.prop()
  };

  Calculator = {
    view: function(ctrl) {
      alert('test 1');
      return m('div', [m('p', 'test')]);
    }
  };

  alert('test 2');

  m.mount(document.getElementById("calculator"), Calculator);

}).call(this);
