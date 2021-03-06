﻿from math import fabs
from pandas import DataFrame
import sys
import argparse
import json


class SMM:
    """
    Wrapper for a prepayment speed in SMM
    """
    def __init__(self, value):
        self.value = value

    def smm(self, month):
        return self.value / 100.0


class CPR:
    def __init__(self, value):
        self.value = value

    def smm(self, month):
        return round(1 - (1 - self.value / 100.0) ** (1.0 / 12.0), 6)


class PSA:
    """
    Wrapper for a prepayment speed in PSA
    """
    def __init__(self, value=100):
        self.value = value

    def cpr(self, month):
        if month < 0:
            raise Exception('Month must be >= 0')

        if month <= 30:
            return 0.0 + 0.2 * month * self.value / 100.0

        return 6 * self.value / 100.0

    def smm(self, month):
        return CPR(self.cpr(month)).smm(month)


def secant_method(fun, x0, x1, tol=0.00005):
    """
    Uses the secant method to find the root of a function

    Parameters
    ----------
    fun : function that accepts one parameter and returns an object of the same type
    x0 : initial guess 1
    x1 : initial guess 2
    tol : minimum difference between two subsequent guesses that must be reached to return a value
    """
    x2 = x1 - fun(x1) * (x1 - x0) / (fun(x1) - fun(x0))
    if fabs(x2 - x1) < tol:
        return x2
    else:
        return secant_method(fun, x1, x2, tol)


def annual_to_monthly(rate):
    """
    Converts an annual interest rate (in percent) to a monthly interest rate (in decimals)
    """
    return rate / 1200.0


def monthly_payment(notional, annual_rate, months):
    """
    Calculates the level payment of a standard, fixed rate mortgage

    Parameters
    ----------
    notional : outstanding loan balance
    annual_rate : annual interest rate (in percent)
    months : remaining term of the loan
    """
    i = annual_to_monthly(annual_rate)
    return notional * (i * (1 + i) ** months) / ((1 + i) ** months - 1)


def amortization_schedule(notional, annual_rate, months, speed=None):
    """
    Calculates an amortization of a loan (returns a pandas DataFrame)

    Parameters
    ----------
    notional : outstanding loan balance
    annual_rate : annual interest rate (in percent)
    months : remaining term of the loan
    speed : (optional) prepayment speed applied to the cashflows.
        Must implement the smm(month) method that returns the SMM for a given period
    """
    monthly_pmt = monthly_payment(notional, annual_rate, months)
    monthly_rate = annual_rate / 1200.0

    periods = []
    balance = notional

    for i in range(months):

        interest = balance * monthly_rate
        principal = monthly_pmt - interest
        balance -= principal
        prepayment = 0

        if speed:
            adj = speed.smm(i + 1)
            prepayment = adj * balance
            balance -= prepayment

            if months - i > 1:
                monthly_pmt = monthly_payment(balance, annual_rate,
                                              months - i - 1)

        if balance < 0:
            principal += balance
            balance -= balance

        period = {}
        period['sched_pmt'] = principal + interest
        period['interest'] = interest
        period['reg_prin'] = principal
        period['total_prin'] = principal + prepayment
        period['prepayment'] = prepayment
        period['balance'] = balance
        period['total_pmt'] = period['total_prin'] + period['interest']

        periods.append(period)

        if balance == 0:
            break

    return DataFrame(periods)


def pv(df, annual_rate):
    """
    Calculates the present value of an amortization schedule given a rate of return

    Parameters
    ----------
    df : a pandas DataFrame
    annual_rate : annual rate of return
    """
    r = annual_to_monthly(annual_rate)
    return (df['total_pmt'] / (1 + r) ** (df.index.values + 1)).sum()


def yld(df, price):
    """
    Calculates the yield of an amortization schedule given a price

    Parameters
    ----------
    df : a pandas DataFrame
    price : price of the loan
    """
    notional = df['total_prin'].sum()

    def yld(r):
        return pv(df, r) / notional * 100 - price

    return secant_method(yld, 0, 1)


def wal(df):
    """
    Calculates the weighted average life of an amortization schedule

    Parameters
    ----------
    df : a pandas DataFrame
    """
    notional = df['total_prin'].sum()
    return (df['total_prin'] / notional * (df.index + 1) / 12.0).sum()


def macaulay_dur(df, price):
    """
    Calculates the Macaulay duration of an amortization schedule at a given price

    Parameters
    ----------
    df : a pandas DataFrame
    price : price of the loan
    """
    notional = df['total_prin'].sum()
    y = yld(df, price)
    r = annual_to_monthly(y)

    return ((df.index.values + 1) * df['total_pmt'] / ((1 + r) **
            (df.index.values + 1))).sum() / (price / 100.0 * notional * 12.0)


def modified_dur(df, price):
    """
    Calculates the Modified duration of an amortization schedule at a given price

    Parameters
    ----------
    df: a pandas DataFrame
    price : price of the loan
    """
    mac_dur = macaulay_dur(df, price)
    y = yld(df, price)
    r = annual_to_monthly(y)
    return mac_dur / (1 + r)


class Mortgage(object):
    """
    An object that represents a mortgage bond.
    """
    def __init__(self, notional, annual_rate, months, speed=None):
        """
        Arguments:
        ----------
        notional: Notional loan amount
        annual_rate: Annual interest rate (non-decimal)
        months: Term of the loan in months

        Optional Arguments:
        -------------------
        speed: Prepayment speed wrapped in a CPR, PSA or SMM object
        """
        self._notional = notional
        self._annual_rate = annual_rate
        self._months = months
        self._speed = speed

    @property
    def notional(self):
        return self._notional

    @property
    def annual_rate(self):
        return self._annual_rate

    @property
    def months(self):
        return self._months

    @property
    def speed(self):
        return self._speed

    _amortization_schedule = None

    @property
    def amortization_schedule(self):
        """
        Lazy loaded property
        """
        if self._amortization_schedule is None:
            self._amortization_schedule = amortization_schedule(self.notional, self.annual_rate,
                                                                self.months, self.speed)
        return self._amortization_schedule

    def yld(self, price):
        return yld(self.amortization_schedule, price)

    def wal(self):
        return wal(self.amortization_schedule)

    def mod_dur(self, price):
        return modified_dur(self.amortization_schedule, price)

    def macaulay_dur(self, price):
        return macaulay_dur(self.amortization_schedule, price)


def main(argv):
    parser = argparse.ArgumentParser()
    # Required args
    parser.add_argument('notional', type=float, help='notional loan amount')
    parser.add_argument('annual_rate', type=float, help='annual interest rate')
    parser.add_argument('months', type=int, help='term of the loan in months')
    parser.add_argument('price', type=float, help='price of loan')
    parser.add_argument('output_path', metavar='O', help='file path of output')

    # Optional args
    parser.add_argument('-speed_type', help='supports CPR, PSA, and SMM', choices=['cpr', 'psa', 'smm'])
    parser.add_argument('-speed_amt', type=float, help='speed level used in conjunction with speed_type')
    parser.add_argument('-output_format', help='file type to export.  If xlsx or csv, only cashflows will be provided',
                        choices=['json', 'xlsx', 'csv'])

    args = vars(parser.parse_args())

    if args['speed_type'] and args['speed_amt']:
        speeds = {'cpr': CPR(args['speed_amt']),
                  'psa': PSA(args['speed_amt']),
                  'smm': SMM(args['speed_amt'])}

        df = amortization_schedule(args['notional'], args['annual_rate'], args['months'],
                                   speed=speeds[args['speed_type']])
    else:
        df = amortization_schedule(args['notional'], args['annual_rate'], args['months'])

    if args['output_format'] == 'json' or not args['output_format']:
        json_obj = {}
        df['period'] = df.index.values
        json_obj['cashflows'] = json.loads(df.to_json(orient='records', double_precision=2))
        json_obj['yield'] = yld(df, args['price'])
        json_obj['wal'] = wal(df)
        json_obj['macaulay_dur'] = macaulay_dur(df, args['price'])
        json_obj['modified_dur'] = modified_dur(df, args['price'])

        f = open(args['output_path'], 'w')
        f.write(json.dumps(json_obj))
        f.close()
    elif args['output_format'] == 'xlsx':
        df.to_excel(args['output_path'])
    elif args['output_format'] == 'csv':
        df.to_csv(args['output_path'])


if __name__ == '__main__':
    main(sys.argv[1:])
