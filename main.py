import logging
from flask import Flask, render_template, request

from yield_calcs import Mortgage, CPR, PSA, SMM


app = Flask(__name__)


@app.route("/")
def hello():
    return render_template("base.html")


@app.route("/api/calculate", methods=["POST"])
def calculate():
    data = request.get_json()

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

    return {
        "amortization_schedule": mtg.amortization_schedule,
        "yield": mtg.yld,
        "wal": mtg.wal,
        "mod_dur": mtg.mod_dur,
        "macaulay_dur": mtg.macaulay_dur
    }


@app.errorhandler(500)
def server_error(e):
    # Log the error and stacktrace.
    logging.exception('An error occurred during a request.')
    return 'An internal error occurred.', 500
