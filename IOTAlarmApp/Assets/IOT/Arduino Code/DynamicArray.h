#ifndef DYNAMIC_ARRAY_H
#define DYNAMIC_ARRAY_H

// DynamicArray class for dynamic array implementation
template <typename T>
class DynamicArray {
public:
    DynamicArray() {
        _capacity = 0;
        _size = 0;
        _array = nullptr;
    }

    ~DynamicArray() {
        clear();
    }

    void push_back(const T& value) {
        if (_size >= _capacity) {
            _resize();
        }
        _array[_size++] = value;
    }

    void pop_back() {
        if (_size > 0) {
            _size--;
        }
    }

    T& operator[](int index) {
        return _array[index];
    }

    int size() const {
        return _size;
    }

    void clear() {
        if (_array != nullptr) {
            delete[] _array;
            _array = nullptr;
        }
        _size = 0;
        _capacity = 0;
    }

private:
    void _resize() {
        int newCapacity = (_capacity == 0) ? 1 : _capacity * 2;
        T* newArray = new T[newCapacity];
        for (int i = 0; i < _size; i++) {
            newArray[i] = _array[i];
        }
        delete[] _array;
        _array = newArray;
        _capacity = newCapacity;
    }

    T* _array;
    int _capacity;
    int _size;
};



#endif