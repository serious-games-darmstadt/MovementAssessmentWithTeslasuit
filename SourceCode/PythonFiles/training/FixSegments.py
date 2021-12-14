from data.DataManager import DataManager

dataManager = DataManager()

for file in dataManager.segmented_files:
    if file != "06_Lunge_Medium.csv" and file != "06_Lunge_Slow.csv" and file != "06_Pushup_Fast.csv" \
            and file != "06_Pushup_Slow.csv" and file != "06_Squat_Fast.csv" and file != "06_Squat_Slow.csv" \
            and file != "07_Lunge_Fast.csv" and file != "07_Lunge_Medium.csv" and file != "07_Lunge_Slow.csv" \
            and file != "07_Pushup_Fast.csv" and file != "07_Pushup_Slow.csv" and file != "07_Squat_Medium.csv" \
            and file != "07_Squat_Slow.csv" and file != "08_Lunge_Slow.csv" and file != "08_Pushup_Medium.csv" \
            and file != "08_Pushup_Slow.csv" and file != "08_Squat_Medium.csv" and file != "08_Squat_Slow.csv" \
            and file != "15_Lunge_Slow.csv" and file != "15_Pushup_Fast.csv" and file != "15_Pushup_Slow.csv" \
            and file != "15_Squat_Fast.csv" and file != "15_Squat_Medium.csv" and file != "15_Squat_Slow.csv":
        continue

    # print(file)

    segmented_data = dataManager.get_data_from_file(dataManager.segmented_data_dir + file, make_numeric=False)
    # audio_segment_data = dataManager.get_data_from_file(dataManager.negativ_lebaled_data_dir + file, make_numeric=False)

    # last_segment = "DATA_START"
    #
    # for index, row in segmented_data.iterrows():
    #     if row["segment"] == "START":
    #         if last_segment == "DATA_START" or last_segment == "END":
    #             last_segment = "START"
    #         else:
    #             print("             ERROR:", file)
    #     if row["segment"] == "END":
    #         if last_segment != "START":
    #             print("             ERROR: ", file)
    #         else:
    #             last_segment = "END"



    segmented_data_starts = segmented_data["segment"] == "START"
    segmented_data_ends = segmented_data["segment"] == "END"
    # # segmented_data["segment"] = "NONE"
    segmented_data.loc[segmented_data_starts, "segment"] = "END"
    segmented_data.loc[segmented_data_ends, "segment"] = "START"



    # audio_segments_start = audio_segment_data["segment"] == "START"
    # audio_segments_end = audio_segment_data["segment"] == "END"
    # segmented_data.loc[audio_segments_start, "segment"] = "NONE"
    # segmented_data.loc[audio_segments_end, "segment"] = "NONE"

    dataManager.save_data_to_path(dataManager.segmented_data_dir + file, segmented_data)