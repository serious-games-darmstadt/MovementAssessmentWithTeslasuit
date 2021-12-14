import numpy as np
import plotly.graph_objs as go
import matplotlib.pyplot as plt

def vector_plot(bins):
    """Plot vectors using plotly"""

    layout = go.Layout(
        scene=dict(
            xaxis=dict(showticklabels=False, visible=True, showgrid=True, showbackground=False, gridcolor="grey", tickvals=[-1, 0, 1], title=dict(text="")),
            yaxis=dict(showticklabels=False, visible=True, showgrid=True, showbackground=False, gridcolor="grey", tickvals=[-1, 0, 1], title=dict(text="")),
            zaxis=dict(showticklabels=False, visible=True, showgrid=True, showbackground=False, gridcolor="grey", tickvals=[-1, 0, 1], title=dict(text="")),
        ),
        showlegend=False
    )

    data = []
    for key, value in bins.items():
        color = "blue"
        if key.__contains__("up"):
            color = "green"
        if key.__contains__("down"):
            color = "orange"
        if key == "up":
            color = "red"
        if key == "down":
            color = "red"

        vector = go.Scatter3d(x = [0, value[0]],
                              y = [0, value[2]],
                              z = [0, value[1]],
                              line=dict(color=color, width=3),
                              mode='lines+text')
        data.append(vector)
        vector = go.Scatter3d(x=[value[0]],
                              y=[value[2]],
                              z=[value[1]],
                              marker=dict(size=0.00001,
                                          color=['blue'],
                                          line=dict(width=1,
                                                    color='DarkSlateGrey')),
                              text=key,
                              textfont=dict(size=20),
                              textposition = "middle center",
                              mode='lines+text')
        data.append(vector)

    fig = go.Figure(data=data,layout=layout)
    fig.show()


vectorBins = {
            "up" : np.array([0, 1, 0]),
            "down" : np.array([0, -1, 0]),

            "front" : np.array([0, 0, -1]),
            "front_up" : np.array([0, 1, -1]),
            "front_down" : np.array([0, -1, -1]),

            "front_left" : np.array([1, 0, -1]),
            "front_left_up" : np.array([1, 1, -1]),
            "front_left_down" : np.array([1, -1, -1]),

            "front_right" : np.array([-1, 0, -1]),
            "front_right_up" : np.array([-1, 1, -1]),
            "front_right_down" : np.array([-1, -1, -1]),

            "left" : np.array([1, 0, 0]),
            "left_up" : np.array([1, 1, 0]),
            "left_down" : np.array([1, -1, 0]),

            "back_left" : np.array([1, 0, 1]),
            "back_left_up" : np.array([1, 1, 1]),
            "back_left_down" : np.array([1, -1, 1]),

            "back" : np.array([0, 0, 1]),
            "back_up" : np.array([0, 1, 1]),
            "back_down" : np.array([0, -1, 1]),

            "back_right" : np.array([-1, 0, 1]),
            "back_right_up" : np.array([-1, 1, 1]),
            "back_right_down" : np.array([-1, -1, 1]),

            "right" : np.array([-1, 0, 0]),
            "right_up" : np.array([-1, 1, 0]),
            "right_down" : np.array([-1, -1, 0]),
        }

vector_plot(vectorBins)