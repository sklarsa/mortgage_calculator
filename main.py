import logging
from flask import Flask, render_template, request, jsonify

from yield_calcs import Mortgage, CPR, PSA, SMM


app = Flask(__name__, static_url_path="/static")


@app.route("/")
def hello():
    return render_template("index.html")


@app.route("/api/calculate", methods=["POST"])
def calculate():
    data = request.get_json()

    logging.info("data %s" % data)

    speed = None
    if "speed" in data:
        speed_type = data["speed"]["speed_type"]
        speed_amt = data["speed"]["speed_amt"]
        if speed_type == "CPR":
            speed = CPR(speed_amt)
        elif speed_type == "PSA":
            speed = PSA(speed_amt)
        elif speed_type == "SMM":
            speed_type = SMM(speed_amt)
        else:
            raise Exception("Speed type {0} not supported".format(speed_type))

    mtg = Mortgage(
        data["notional"],
        data["rate"],
        data["months"],
        speed=speed
    )

    if "price" not in data:
        price = 100.0
    else:
        price = data["price"]

    return jsonify(**{
        "amortization_schedule": mtg.amortization_schedule.to_json(orient="records"),
        "yield": mtg.yld(price),
        "wal": mtg.wal(),
        "mod_dur": mtg.mod_dur(price),
        "macaulay_dur": mtg.macaulay_dur(price)
    })


@app.errorhandler(500)
def server_error(e):
    # Log the error and stacktrace.
    logging.exception('An error occurred during a request.')
    return 'An internal error occurred.', 500


if __name__ == "__main__":
    app.run()
