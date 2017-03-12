source venv/bin/activate

FLASK_APP=main.py
FLASK_DEBUG=1

gulp watch &
PID1=$!
python main.py
PID2=$!

function ctrl_c(){
    kill PID1
    kill PID2
}
