import styles from "../../styles/StorageChanger.module.css"
import {useState} from "react";
import {useDispatch} from "react-redux";
import {changeStorage, fetchCategories, fetchToDos, initialState} from "@/features/todolist/ToDoListSlice.js";

function StorageChanger() {
    const [storage, setStorage] = useState(initialState.storage);

    const dispatch = useDispatch();

    function handleStorage(e) {
        const newStorage = e.target.value;

        setStorage(newStorage);

        dispatch(changeStorage(newStorage));
        dispatch(fetchCategories());
        dispatch(fetchToDos())
    }

    return (
        <select onChange={handleStorage} className={styles.select}>
            <option value="XmlStorage">Xml Storage</option>
            <option value="DbStorage">Db Storage</option>
        </select>
    );
}

export default StorageChanger;