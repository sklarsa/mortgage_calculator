class Mortgage
    constructor: (obj) ->
        @notional = m.prop(obj.notional)
        @rate = m.prop(obj.rate)
        @months = m.prop(obj.months)
        @speed_type = m.prop(obj.speed_type)
        @speed_amt = m.prop(obj.speed_amt)
        @price = m.prop(obj.price)

class Result
    constructor: (obj) ->
        @amortization_schedule = m.prop(c for c in JSON.parse(obj.amortization_schedule))
        @yield = m.prop(obj.yield)
        @wal = m.prop(obj.wal)
        @mod_dur = m.prop(obj.mod_dur)
        @macaulay_dur = m.prop(obj.macaulay_dur)

vm =
    mortgage: m.prop(new Mortgage({
        notional: 1000000
        rate: 5
        months: 360
        speed_type: "CPR"
        speed_amt: 0
        price: 100
    }))
    result: m.prop()

AnalyticsIO =
    controller: ->
        submit: (e) ->
            e.preventDefault()

            m.request(
                method: "POST"
                url: "api/calculate"
                type: Result
                data: vm.mortgage()
            ).then (result) ->
                vm.result(result)
    view: (ctrl) ->
        m 'div', [
            m '.row', [
                m '.col-md-6', [
                    m 'form', [
                        m '.form-group', [
                            m 'label', {for: 'notional'}, 'Notional'
                            m 'input', {
                                type: 'number'
                                class: 'form-control'
                                id: 'notional'
                                value: vm.mortgage().notional()
                                onchange: m.withAttr("value", vm.mortgage().notional)
                            }
                        ]
                        m '.form-group', [
                            m 'label', {for: 'rate'}, 'Rate'
                            m 'input', {
                                type: 'number'
                                class: 'form-control'
                                id: 'rate'
                                value: vm.mortgage().rate()
                                onchange: m.withAttr("value", vm.mortgage().rate)
                            }
                        ]
                        m '.form-group', [
                            m 'label', {for: 'months'}, 'Months'
                            m 'input', {
                                type: 'number'
                                class: 'form-control'
                                id: 'months'
                                value: vm.mortgage().months()
                                onchange: m.withAttr("value", vm.mortgage().months)
                            }
                        ]
                        m '.form-group', [
                            m 'label', {for: 'speed_type'}, 'Speed Type'
                            m 'select', {
                                class: 'form-control'
                                id: 'speed_type'
                                value: vm.mortgage().speed_type()
                                onchange: m.withAttr("value", vm.mortgage().speed_type)
                            }, [
                                m 'option', 'CPR'
                                m 'option', 'PSA'
                                m 'option', 'SMM'
                            ]
                        ]
                        m '.form-group', [
                            m 'label', {for: 'speed_amt'}, 'Speed Amount'
                            m 'input', {
                                type: 'number'
                                class: 'form-control'
                                id: 'speed_amt'
                                value: vm.mortgage().speed_amt()
                                onchange: m.withAttr("value", vm.mortgage().speed_amt)
                            }
                        ]
                        m '.form-group', [
                            m 'label', {for: 'price'}, 'Price'
                            m 'input', {
                                type: 'number'
                                class: 'form-control'
                                id: 'price'
                                value: vm.mortgage().price()
                                onchange: m.withAttr("value", vm.mortgage().price)
                            }
                        ]
                        m 'button', {type: 'submit', class: 'btn btn-default', onclick: ctrl.submit}, 'Get Bond Analytics'
                    ]
                ]
                m '.col-md-6', [
                    m 'h4', 'Results:'
                    m 'table', {class: 'table table-striped table-bordered'}, [
                        m 'tbody', [
                            m 'tr', [
                                m 'td', 'Yield'
                                m 'td', if vm.result()? then vm.result().yield() else ''
                            ]
                            m 'tr', [
                                m 'td', 'WAL'
                                m 'td', if vm.result()? then vm.result().wal() else ''
                            ]
                            m 'tr', [
                                m 'td', 'Mod Dur'
                                m 'td', if vm.result()? then vm.result().mod_dur() else ''
                            ]
                            m 'tr', [
                                m 'td', 'Macaulay Dur'
                                m 'td', if vm.result()? then vm.result().macaulay_dur() else ''
                            ]
                        ]
                    ]
                ]
            ]
        ]

AmortizationSchedule =
    view: (ctrl) ->
        m 'div', [
            if vm.result()?
                m 'div', [
                    m 'h3', 'Amortization Schedule'
                    m 'table', {class: 'table table-striped'}, [
                        m 'thead', [
                            m 'tr', [
                                m 'td', 'Period'
                                m 'td', 'Sched Pmt'
                                m 'td', 'Interest'
                                m 'td', 'Reg Prin'
                                m 'td', 'Prepayment'
                                m 'td', 'Total Prin'
                                m 'td', 'Balance'
                                m 'td', 'Total Pmt'
                            ]
                        ]
                        m 'tbody', [
                            for p, i in vm.result().amortization_schedule()
                                m 'tr', [
                                    m 'td', i
                                    m 'td', p.sched_pmt
                                    m 'td', p.interest
                                    m 'td', p.reg_prin
                                    m 'td', p.prepayment
                                    m 'td', p.total_prin
                                    m 'td', p.balance
                                    m 'td', p.total_pmt
                                ]
                        ]
                    ]
                ]
        ]

Calculator =
    view: (ctrl) ->
        m 'div', [
            m.component AnalyticsIO
            m.component AmortizationSchedule
        ]


m.mount document.getElementById("calculator"), Calculator