(function() {
  var Calculator, Input, Mortgage, Output, Result, vm;

  Mortgage = (function() {
    function Mortgage(obj) {
      this.notional = m.prop(obj.notional);
      this.rate = m.prop(obj.rate);
      this.months = m.prop(obj.months);
      this.speed_type = m.prop(obj.speed_type);
      this.speed_amt = m.prop(obj.speed_amt);
      this.price = m.prop(obj.price);
    }

    return Mortgage;

  })();

  Result = (function() {
    function Result(obj) {
      debugger;
      var c;
      this.amortization_schedule = m.prop((function() {
        var i, len, ref, results;
        ref = JSON.parse(obj.amortization_schedule);
        results = [];
        for (i = 0, len = ref.length; i < len; i++) {
          c = ref[i];
          results.push(c);
        }
        return results;
      })());
      this["yield"] = m.prop(obj["yield"]);
      this.wal = m.prop(obj.wal);
      this.mod_dur = m.prop(obj.mod_dur);
      this.macaulay_dur = m.prop(obj.macaulay_dur);
    }

    return Result;

  })();

  vm = {
    mortgage: m.prop(new Mortgage({
      notional: 1000000,
      rate: 5,
      months: 360,
      speed_type: "CPR",
      speed_amt: 0,
      price: 100
    })),
    result: m.prop()
  };

  Input = {
    controller: function() {
      return {
        submit: function(e) {
          e.preventDefault();
          return m.request({
            method: "POST",
            url: "api/calculate",
            type: Result,
            data: vm.mortgage()
          }).then(function(result) {
            return vm.result(result);
          });
        }
      };
    },
    view: function(ctrl) {
      return m('div', [
        m('.row', [
          m('.col-md-6 col-md-offset-3', [
            m('form', [
              m('.form-group', [
                m('label', {
                  "for": 'notional'
                }, 'Notional'), m('input', {
                  type: 'number',
                  "class": 'form-control',
                  id: 'notional',
                  value: vm.mortgage().notional()
                })
              ]), m('.form-group', [
                m('label', {
                  "for": 'rate'
                }, 'Rate'), m('input', {
                  type: 'number',
                  "class": 'form-control',
                  id: 'rate',
                  value: vm.mortgage().rate()
                })
              ]), m('.form-group', [
                m('label', {
                  "for": 'months'
                }, 'Months'), m('input', {
                  type: 'number',
                  "class": 'form-control',
                  id: 'months',
                  value: vm.mortgage().months()
                })
              ]), m('.form-group', [
                m('label', {
                  "for": 'speed_type'
                }, 'Speed Type'), m('select', {
                  "class": 'form-control',
                  id: 'speed_type',
                  value: vm.mortgage().speed_type()
                }, [m('option', 'CPR'), m('option', 'PSA'), m('option', 'SMM')])
              ]), m('.form-group', [
                m('label', {
                  "for": 'speed_amt'
                }, 'Speed Amount'), m('input', {
                  type: 'number',
                  "class": 'form-control',
                  id: 'speed_amt',
                  value: vm.mortgage().speed_amt()
                })
              ]), m('.form-group', [
                m('label', {
                  "for": 'price'
                }, 'Price'), m('input', {
                  type: 'number',
                  "class": 'form-control',
                  id: 'price',
                  value: vm.mortgage().price()
                })
              ]), m('button', {
                type: 'submit',
                "class": 'btn btn-default',
                onclick: ctrl.submit
              }, 'Get Bond Analytics')
            ])
          ])
        ])
      ]);
    }
  };

  Output = {
    view: function(ctrl) {
      var p;
      return m('div', [
        vm.result() != null ? m('div', [
          m('h3', 'Amortization Schedule'), m('table', {
            "class": 'table table-striped'
          }, [
            m('thead', [m('tr', [m('td', 'Sched Pmt'), m('td', 'Interest'), m('td', 'Reg Prin'), m('td', 'Prepayment'), m('td', 'Total Prin'), m('td', 'Balance'), m('td', 'Total Pmt')])]), m('tbody', [
              (function() {
                var i, len, ref, results;
                ref = vm.result().amortization_schedule();
                results = [];
                for (i = 0, len = ref.length; i < len; i++) {
                  p = ref[i];
                  console.log(p);
                  results.push(m('tr', [m('td', p.sched_pmt), m('td', p.interest), m('td', p.reg_prin), m('td', p.prepayment), m('td', p.total_prin), m('td', p.balance), m('td', p.total_pmt)]));
                }
                return results;
              })()
            ])
          ])
        ]) : void 0
      ]);
    }
  };

  Calculator = {
    view: function(ctrl) {
      return m('div', [m.component(Input), m.component(Output)]);
    }
  };

  m.mount(document.getElementById("calculator"), Calculator);

}).call(this);
