from datetime import tzinfo
import json
import dateutil.parser as dt
import matplotlib 
import matplotlib.pyplot as plt
import matplotlib.dates as mdates

from dateutil import tz
from tzlocal import get_localzone

input_file = open("out1_TSLA.txt", "r")


def convert_date_time_to_utc(datetime):
    utc_zone = tz.gettz('UTC')
    local_tz = get_localzone()

    datetime = datetime.replace(tzinfo = local_tz)

    return datetime.astimezone(utc_zone)

benchmark_data = json.load(input_file)
fig, ax = plt.subplots()
plt.gca().xaxis.set_major_formatter(mdates.DateFormatter('%m/%d/%Y %H:%M'))
plt.gca().xaxis.set_major_locator(mdates.DayLocator())


i = 0

for result in benchmark_data:
    print(result['Algorithm'])
    print('RMSE: ' + str(result['ComputedStatistics']['RMSE']))
    print('MAE: ' + str(result['ComputedStatistics']['MAE']))

    x_label = [dt.parse(y['Date']) for y in result['ForecastedPrices']]
    y_label = [y['Price'] for y in result['ForecastedPrices']]

    if i == 0:
        x_actual = [convert_date_time_to_utc(dt.parse(y['Date'])) for y in result['Actuals']]
        y_actual = [y['Price'] for y in result['Actuals']]
        ax.plot(x_actual, y_actual, label='Actuals' + result['Ticker'])

    # if i == 1:
    #     x_actual = [dt.parse(y['Date']) for y in result['Dataset']]
    #     y_actual = [y['Price'] for y in result['Dataset']]
    #     ax.plot(x_actual, y_actual)

    i += 1

    ax.plot(x_label, y_label, label=result['Algorithm'])
plt.legend()
plt.show()
