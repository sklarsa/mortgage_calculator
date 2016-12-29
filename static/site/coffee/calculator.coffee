class Mortgage
    constructor: (obj) ->
        @notional = m.prop(1000000)
        @rate = m.prop(5)
        @months = m.prop(360)
        @speed_type = m.prop("CPR")
        @speed_amt = m.prop(0)
        @price = m.prop(100)

class Result
    constructor: (obj) ->
        @amortization_schedule = m.prop()
        @yield = m.prop()
        @wal = m.prop()
        @mod_dur = m.prop()
        @macaulay_dur = m.prop()

vm =
    mortgage: m.prop(new Mortgage())
    result: m.prop(new Result())

Input =
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
                debugger
    view: (ctrl) ->
        m 'div', [
            m '.row', [
                m '.col-md-6 col-md-offset-3', [
                    m 'form', [
                        m '.form-group', [
                            m 'label', {for: 'notional'}, 'Notional'
                            m 'input', {type: 'number', class: 'form-control', id: 'notional', value: vm.mortgage().notional()}
                        ]
                        m '.form-group', [
                            m 'label', {for: 'rate'}, 'Rate'
                            m 'input', {type: 'number', class: 'form-control', id: 'rate', value: vm.mortgage().rate()}
                        ]
                        m '.form-group', [
                            m 'label', {for: 'months'}, 'Months'
                            m 'input', {type: 'number', class: 'form-control', id: 'months', value: vm.mortgage().months()}
                        ]
                        m '.form-group', [
                            m 'label', {for: 'speed_type'}, 'Speed Type'
                            m 'select', {class: 'form-control', id: 'speed_type', value: vm.mortgage().speed_type()}, [
                                m 'option', 'CPR'
                                m 'option', 'PSA'
                                m 'option', 'SMM'
                            ]
                        ]
                        m '.form-group', [
                            m 'label', {for: 'speed_amt'}, 'Speed Amount'
                            m 'input', {type: 'number', class: 'form-control', id: 'speed_amt', value: vm.mortgage().speed_amt()}
                        ]
                        m '.form-group', [
                            m 'label', {for: 'price'}, 'Price'
                            m 'input', {type: 'number', class: 'form-control', id: 'price', value: vm.mortgage().price()}
                        ]
                        m 'button', {type: 'submit', class: 'btn btn-default', onclick: ctrl.submit}, 'Get Bond Analytics'
                    ]
                ]
            ]
        ]


Calculator =
    view: (ctrl) ->
        m.component Input

m.mount document.getElementById("calculator"), Calculator